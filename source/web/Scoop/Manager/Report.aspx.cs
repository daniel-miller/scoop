using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Scoop
{
    public partial class ReportPage : ManagerPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var organization = Request.QueryString["o"];

            var package = Request.QueryString["p"];

            var packagePath = AppSettings.GetPhysicalPathToPackage(Context, organization, package);

            string manifestPath = Path.Combine(packagePath, "imsmanifest.xml");

            if (!File.Exists(manifestPath))
                return;

            var xmlContent = File.ReadAllText(manifestPath);

            var parser = new ScormManifestParser();

            var resources = parser.ParseManifest(xmlContent);

            var sb = new StringBuilder();

            // Display parsed resources
            foreach (var resource in resources)
            {
                sb.AppendLine($"ID: {resource.Identifier}");
                sb.AppendLine($"Href: {resource.Href}");
                sb.AppendLine($"Title: {resource.Title}");
                sb.AppendLine("---");
            }

            // Get launch pages
            var launchPages = parser.GetLaunchPages(xmlContent);
            sb.AppendLine("\nLaunch Pages:");
            foreach (var page in launchPages)
            {
                var indent = new string(' ', page.Level * 2);
                sb.AppendLine($"{indent}- {page.Title} ({page.Href}){(string.IsNullOrEmpty(page.Parameters) ? "" : " " + page.Parameters)}");
            }

            // Get hierarchical launch pages
            var hierarchicalPages = parser.GetLaunchPagesHierarchical(xmlContent);
            sb.AppendLine("\nHierarchical Launch Pages:");
            DisplayHierarchicalPages(hierarchicalPages, 0, sb);

            // Generate JavaScript
            sb.AppendLine("\nGenerated JavaScript for Resources:");
            sb.AppendLine(parser.GenerateJavaScriptWithFunctions(resources));

            sb.AppendLine("\nGenerated JavaScript for Launch Pages:");
            sb.AppendLine(parser.GenerateJavaScriptLaunchPages(launchPages));

            output.InnerText = sb.ToString();
        }

        private static void DisplayHierarchicalPages(List<ScormLaunchPage> pages, int level, StringBuilder sb)
        {
            foreach (var page in pages)
            {
                var indent = new string(' ', level * 2);
                var launchable = !string.IsNullOrEmpty(page.Href) ? $" -> {page.Href}" : " [Section]";
                sb.AppendLine($"{indent}- {page.Title}{launchable}");

                if (page.Children.Any())
                {
                    DisplayHierarchicalPages(page.Children, level + 1, sb);
                }
            }
        }
    }
}