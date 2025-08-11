using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Xml;

using Newtonsoft.Json;

namespace Scoop
{
    public class PackageProgress : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            var action = context.Request.QueryString["action"];

            var packageNumber = Convert.ToInt32(context.Request.QueryString["packageNumber"]);

            var reader = new PackageReader();

            var info = reader.RetrievePackage(packageNumber);

            var learnerId = Guid.Parse(context.Request.QueryString["learnerId"]);

            context.Response.ContentType = "application/json";

            try
            {
                switch (action)
                {
                    case "load":
                        LoadScormData(context, packageNumber, learnerId);
                        break;

                    case "save":
                        SaveScormData(context, packageNumber, learnerId);
                        break;

                    case "getManifest":
                        GetManifestData(context, info);
                        break;

                    case "getAllPackages":
                        GetAllPackages(context, learnerId);
                        break;

                    default:
                        context.Response.Write(JsonConvert.SerializeObject(new { error = "Invalid action" }));
                        break;
                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.Write(JsonConvert.SerializeObject(new { error = ex.Message }));
            }
        }

        private void LoadScormData(HttpContext context, int package, Guid learner)
        {
            var reader = new ProgressReader();

            var data = reader.Load(package, learner);

            // If no data exists, return default values
            if (data.Count == 0)
            {
                data["cmi.core.lesson_status"] = "not attempted";
                data["cmi.core.entry"] = "ab-initio";
                data["cmi.core.lesson_mode"] = "normal";
                data["cmi.core.credit"] = "credit";
                data["cmi.core.total_time"] = "0000:00:00";
                data["cmi.suspend_data"] = "";
                data["cmi.launch_data"] = "";
                data["cmi.core.lesson_location"] = "";
                data["cmi.core.score.raw"] = "";
                data["cmi.core.score.min"] = "";
                data["cmi.core.score.max"] = "";
            }

            context.Response.Write(JsonConvert.SerializeObject(data));
        }

        private void SaveScormData(HttpContext context, int package, Guid learner)
        {
            string requestBody = new StreamReader(context.Request.InputStream).ReadToEnd();

            Dictionary<string, string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(requestBody);

            if (data == null)
            {
                context.Response.StatusCode = 400;
                context.Response.Write(JsonConvert.SerializeObject(new { error = "Invalid data" }));
                return;
            }

            var writer = new ProgressWriter();

            if (writer.Save(package, learner, data))
                context.Response.Write(JsonConvert.SerializeObject(new { success = true, message = "Data saved successfully" }));
        }

        private void GetManifestData(HttpContext context, CourseInfo package)
        {
            try
            {
                string packagePath = AppSettings.GetPhysicalPathToPackage(package.OrganizationSlug, package.PackageSlug);

                string manifestPath = Path.Combine(packagePath, "imsmanifest.xml");

                if (!File.Exists(manifestPath))
                {
                    context.Response.StatusCode = 404;
                    context.Response.Write(JsonConvert.SerializeObject(new { error = "Manifest not found" }));
                    return;
                }

                // Parse manifest and return SCO information
                List<Dictionary<string, string>> scoList = new List<Dictionary<string, string>>();
                XmlDocument doc = new XmlDocument();
                doc.Load(manifestPath);

                XmlNamespaceManager nsManager = new XmlNamespaceManager(doc.NameTable);
                nsManager.AddNamespace("imscp", "http://www.imsproject.org/xsd/imscp_rootv1p1p2");
                nsManager.AddNamespace("adlcp", "http://www.adlnet.org/xsd/adlcp_rootv1p2");

                // Get all resources
                XmlNodeList resources = doc.SelectNodes("//imscp:resource", nsManager);
                if (resources != null)
                {
                    foreach (XmlNode resource in resources)
                    {
                        if (resource.Attributes != null)
                        {
                            XmlAttribute hrefAttr = resource.Attributes["href"];
                            XmlAttribute identifierAttr = resource.Attributes["identifier"];
                            XmlAttribute typeAttr = resource.Attributes["type"];

                            string href = hrefAttr != null ? hrefAttr.Value : "";
                            string identifier = identifierAttr != null ? identifierAttr.Value : "";
                            string type = typeAttr != null ? typeAttr.Value : "";

                            if (!string.IsNullOrEmpty(href) && type == "webcontent")
                            {
                                Dictionary<string, string> sco = new Dictionary<string, string>();
                                sco["identifier"] = identifier;
                                sco["href"] = href;
                                sco["type"] = type;
                                scoList.Add(sco);
                            }
                        }
                    }
                }

                // Get organization structure
                XmlNode organization = doc.SelectSingleNode("//imscp:organization", nsManager);
                string title = "SCORM Course";
                if (organization != null)
                {
                    XmlNode titleNode = organization.SelectSingleNode("imscp:title", nsManager);
                    if (titleNode != null)
                    {
                        title = titleNode.InnerText;
                    }
                }

                Dictionary<string, object> manifestData = new Dictionary<string, object>();
                manifestData["title"] = title;
                manifestData["scos"] = scoList;
                manifestData["package"] = package;

                context.Response.Write(JsonConvert.SerializeObject(manifestData));
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.Write(JsonConvert.SerializeObject(new { error = ex.Message }));
            }
        }

        private void GetAllPackages(HttpContext context, Guid learnerId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Engine"].ConnectionString;

            List<Dictionary<string, object>> packages = new List<Dictionary<string, object>>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                SELECT 
                    p.PackageNumber,
                    p.Title,
                    p.Description,
                    p.Version,
                    p.UploadDate,
                    ISNULL(sd.Status, 'not attempted') as Status,
                    ISNULL(sd.Score, '') as Score,
                    ISNULL(sd.TotalTime, '0000:00:00') as TotalTime
                FROM scoop.ScoPackage p
                LEFT JOIN (
                    SELECT 
                        PackageNumber,
                        MAX(CASE WHEN ProgressKey = 'cmi.core.lesson_status' THEN ProgressValue END) as Status,
                        MAX(CASE WHEN ProgressKey = 'cmi.core.score.raw' THEN ProgressValue END) as Score,
                        MAX(CASE WHEN ProgressKey = 'cmi.core.total_time' THEN ProgressValue END) as TotalTime
                    FROM scoop.ScoProgress
                    WHERE LearnerId = @LearnerId
                    GROUP BY PackageNumber
                ) sd ON p.PackageNumber = sd.PackageNumber
                WHERE p.IsActive = 1
                ORDER BY p.UploadDate DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@LearnerId", learnerId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, object> package = new Dictionary<string, object>();
                            package["packageNumber"] = reader["PackageNumber"].ToString();
                            package["title"] = reader["Title"].ToString();
                            package["description"] = reader["Description"].ToString();
                            package["version"] = reader["Version"].ToString();
                            package["uploadDate"] = Convert.ToDateTime(reader["UploadDate"]).ToString("yyyy-MM-dd");
                            package["status"] = reader["Status"].ToString();
                            package["score"] = reader["Score"].ToString();
                            package["totalTime"] = reader["TotalTime"].ToString();
                            packages.Add(package);
                        }
                    }
                }
            }

            context.Response.Write(JsonConvert.SerializeObject(packages));
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}