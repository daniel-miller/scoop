using System;
using System.IO;
using System.Linq;
using System.Xml;

namespace Scoop
{
    public partial class PackagePlay : LearnerPage
    {
        protected string ExitUrl { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                return;

            var organization = Request.QueryString["o"];

            var package = Request.QueryString["p"];

            if (string.IsNullOrEmpty(package))
            {
                Response.Redirect(Navigator.ListPath);
            }

            // Float the navigation bar to the right, if requested

            var navigation = Request.QueryString["n"];

            if (Page.Header != null)
            {
                var position = navigation == "right"
                    ? "right"
                    : "left";

                var style = new System.Web.UI.HtmlControls.HtmlGenericControl();
                style.InnerHtml = $"<style>.floating-status {{ {position}: 5px !important; }}</style>";
                Page.Header.Controls.Add(style);
            }

            SetExitUrl(organization);

            // Store in hidden fields for JavaScript access

            hdnOrganizationSlug.Value = organization;

            hdnPackageSlug.Value = package;

            hdnLearnerId.Value = CurrentLearnerId.ToString();

            // Load course information

            LoadCourseInfo(AppSettings.PackageFolder, organization, package);

            var reader = new PackageReader();

            var number = reader.RetrievePackage(organization, package).PackageNumber;

            hdnPackageNumber.Value = number.ToString();
        }

        protected void SetExitUrl(string organizationSlug)
        {
            var exitUrl = Request.QueryString["exitUrl"];

            ExitUrl = exitUrl != null
                ? DecodeBase64(exitUrl)
                : ResolveUrl($"~/{organizationSlug}");
        }

        private void LoadCourseInfo(string packageRoot, string organizationSlug, string packageSlug)
        {
            try
            {
                string packagePath = AppSettings.GetPhysicalPathToPackage(Context, organizationSlug, packageSlug);

                string manifestPath = Path.Combine(packagePath, "imsmanifest.xml");

                if (File.Exists(manifestPath))
                {
                    XmlDocument manifest = new XmlDocument();
                    manifest.Load(manifestPath);

                    // Get course title from manifest
                    XmlNamespaceManager nsManager = new XmlNamespaceManager(manifest.NameTable);
                    nsManager.AddNamespace("imscp", "http://www.imsproject.org/xsd/imscp_rootv1p1p2");

                    XmlNode titleNode = manifest.SelectSingleNode("//imscp:title", nsManager);
                    if (titleNode != null)
                    {
                        lblCourseTitle.Text = titleNode.InnerText;
                    }

                    hdnManifestPath.Value = manifestPath;

                    var xmlContent = File.ReadAllText(manifestPath);

                    ScormManifestParser parser = new ScormManifestParser();

                    var pages = parser.GetLaunchPages(xmlContent);

                    var page = pages.FirstOrDefault();

                    if (page != null)
                    {
                        hdnPackageHref.Value = page.Href;
                        hdnPackageRoot.Value = packageRoot;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
                System.Diagnostics.Debug.WriteLine($"Error loading course info: {ex.Message}");
            }
        }

        private static string DecodeBase64(string encoded)
        {
            var encodedBytes = Convert.FromBase64String(encoded);
            return System.Text.Encoding.UTF8.GetString(encodedBytes);
        }
    }
}