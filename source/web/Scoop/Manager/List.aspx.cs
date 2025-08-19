using System;
using System.IO;
using System.IO.Compression;
using System.Web.UI.WebControls;
using System.Xml;

namespace Scoop
{
    public partial class PackageList : ManagerPage
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            rptCourses.ItemDataBound += RptCourses_ItemDataBound;
        }

        private void RptCourses_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var launch = (HyperLink)e.Item.FindControl("btnLaunch");

            var report = (HyperLink)e.Item.FindControl("btnReport");

            var item = (CourseInfo)e.Item.DataItem;

            launch.NavigateUrl = Navigator.LaunchPath(item.OrganizationSlug, item.PackageSlug);

            report.NavigateUrl = Navigator.ReportPath(item.OrganizationSlug, item.PackageSlug);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Form.Action = Request.RawUrl;

            LearnLink.HRef = Navigator.RepoUrl;

            if (IsPostBack)
                return;

            var name = AppSettings.OrganizationName;

            LibraryHeading.InnerText = $"{name}";

            LoadCourses();

            LoadStatistics();
        }

        private void LoadCourses()
        {
            var reader = new PackageReader();

            var courses = reader.CollectCourses(CurrentOrganizationSlug, CurrentManagerId);

            rptCourses.DataSource = courses;

            rptCourses.DataBind();
        }

        private void LoadStatistics()
        {
            var reader = new ReportReader();

            var organization = CurrentOrganizationSlug;

            var statistics = reader.RetrieveOrganizationStatistics(organization);

            PackageCount.Text = statistics.PackageCount.ToString();

            LearnerCount.Text = statistics.LearnerCount.ToString();

            RegistrationCount.Text = statistics.RegistrationCount.ToString();
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (fileUpload.HasFile)
            {
                try
                {
                    var organization = CurrentOrganizationSlug;

                    var package = Path.GetFileNameWithoutExtension(fileUpload.FileName).ToKebabCase();

                    var packagePath = AppSettings.GetPhysicalPathToPackage(Context, organization, package);

                    // Create directory for package
                    Directory.CreateDirectory(packagePath);

                    // Save and extract ZIP file
                    string zipPath = Path.Combine(packagePath, "package.zip");
                    fileUpload.SaveAs(zipPath);
                    ZipFile.ExtractToDirectory(zipPath, packagePath);
                    File.Delete(zipPath);

                    // Parse manifest to get course info
                    CourseInfo courseInfo = ParseManifest(packagePath);

                    courseInfo.OrganizationSlug = AppSettings.OrganizationSlug;
                    courseInfo.PackageSlug = package;
                    courseInfo.PackageSizeInKB = BytesToKilobytes(fileUpload.PostedFile.ContentLength);

                    // Save to database
                    SavePackageInfo(courseInfo);

                    lblUploadStatus.Text = "Package uploaded successfully!";
                    lblUploadStatus.ForeColor = System.Drawing.Color.Green;

                    Response.Redirect(Request.RawUrl);
                }
                catch (Exception ex)
                {
                    lblUploadStatus.Text = "Error uploading package: " + ex.Message;
                    lblUploadStatus.ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        private int BytesToKilobytes(long bytes)
        {
            double kilobytes = bytes / 1000.0;
            return (int)Math.Round(kilobytes, 0);
        }

        private CourseInfo ParseManifest(string packagePath)
        {
            CourseInfo info = new CourseInfo();

            string manifestPath = Path.Combine(packagePath, "imsmanifest.xml");

            if (File.Exists(manifestPath))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(manifestPath);

                XmlNamespaceManager nsManager = new XmlNamespaceManager(doc.NameTable);
                nsManager.AddNamespace("imscp", "http://www.imsproject.org/xsd/imscp_rootv1p1p2");
                nsManager.AddNamespace("adlcp", "http://www.adlnet.org/xsd/adlcp_rootv1p2");

                // Get title
                XmlNode titleNode = doc.SelectSingleNode("//imscp:title", nsManager);
                if (titleNode != null)
                    info.Title = titleNode.InnerText;

                // Get description
                XmlNode descNode = doc.SelectSingleNode("//imscp:description", nsManager);
                if (descNode != null)
                    info.Description = descNode.InnerText;

                // Get version
                XmlNode versionNode = doc.SelectSingleNode("//imscp:metadata/imscp:schemaversion", nsManager);
                if (versionNode != null)
                    info.Version = versionNode.InnerText;
            }

            return info;
        }

        private void SavePackageInfo(CourseInfo courseInfo)
        {
            var writer = new PackageWriter();

            writer.SavePackageInfo(courseInfo, CurrentManagerEmail);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            var btn = (LinkButton)sender;

            var organization = CurrentOrganizationSlug;

            var package = btn.CommandArgument;

            // Delete from database

            var writer = new PackageWriter();

            writer.Delete(organization, package, CurrentManagerEmail);

            // Delete files

            var packagePath = AppSettings.GetPhysicalPathToPackage(Context, organization, package);

            if (Directory.Exists(packagePath))
                Directory.Delete(packagePath, true);

            Response.Redirect(Request.RawUrl);
        }
    }
}