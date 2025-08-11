using System;
using System.IO;
using System.Linq;
using System.Xml;

namespace Scoop
{
    public partial class PackagePlay : LearnerPage
    {
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
    }
}