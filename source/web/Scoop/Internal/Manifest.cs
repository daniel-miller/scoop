using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Scoop
{
    public class ScormResource
    {
        public string Identifier { get; set; }
        public string Href { get; set; }
        public string Title { get; set; }
    }

    public class ScormLaunchPage
    {
        public string Identifier { get; set; }
        public string ResourceIdentifier { get; set; }
        public string Href { get; set; }
        public string Title { get; set; }
        public string Parameters { get; set; }
        public int Level { get; set; }
        public string ParentIdentifier { get; set; }
        public List<ScormLaunchPage> Children { get; set; } = new List<ScormLaunchPage>();
    }

    public class ScormManifestParser
    {
        public List<ScormResource> ParseManifest(string xmlContent)
        {
            var document = XDocument.Parse(xmlContent);
            var resources = new List<ScormResource>();

            // Define namespaces used in the manifest
            XNamespace imscp = "http://www.imsproject.org/xsd/imscp_rootv1p1p2";
            XNamespace adlcp = "http://www.adlnet.org/xsd/adlcp_rootv1p2";

            // Get all resources from the resources section
            var resourceElements = document
                .Element(imscp + "manifest")?
                .Element(imscp + "resources")?
                .Elements(imscp + "resource");

            if (resourceElements == null)
                return resources;

            // Build a dictionary to map identifierref to titles from the organization structure
            var titleMap = BuildTitleMap(document, imscp);

            foreach (var resourceElement in resourceElements)
            {
                var identifier = resourceElement.Attribute("identifier")?.Value;
                var href = resourceElement.Attribute("href")?.Value;

                // Skip common_files and other non-content resources
                if (string.IsNullOrEmpty(identifier) || identifier == "common_files")
                    continue;

                var resource = new ScormResource
                {
                    Identifier = identifier,
                    Href = href,
                    Title = titleMap.ContainsKey(identifier) ? titleMap[identifier] : identifier
                };

                resources.Add(resource);
            }

            return resources;
        }

        private Dictionary<string, string> BuildTitleMap(XDocument document, XNamespace imscp)
        {
            var titleMap = new Dictionary<string, string>();

            // Navigate through the organization structure to find titles
            var organization = document
                .Element(imscp + "manifest")?
                .Element(imscp + "organizations")?
                .Element(imscp + "organization");

            if (organization != null)
            {
                ExtractTitlesFromItems(organization.Elements(imscp + "item"), titleMap, imscp);
            }

            return titleMap;
        }

        private void ExtractTitlesFromItems(IEnumerable<XElement> items, Dictionary<string, string> titleMap, XNamespace imscp)
        {
            foreach (var item in items)
            {
                var identifierRef = item.Attribute("identifierref")?.Value;
                var title = item.Element(imscp + "title")?.Value;

                // If this item has an identifierref, it references a resource
                if (!string.IsNullOrEmpty(identifierRef) && !string.IsNullOrEmpty(title))
                {
                    titleMap[identifierRef] = title;
                }

                // Recursively process child items
                var childItems = item.Elements(imscp + "item");
                if (childItems.Any())
                {
                    ExtractTitlesFromItems(childItems, titleMap, imscp);
                }
            }
        }

        public List<ScormLaunchPage> GetLaunchPages(string xmlContent)
        {
            var document = XDocument.Parse(xmlContent);
            var launchPages = new List<ScormLaunchPage>();

            // Define namespaces used in the manifest
            XNamespace imscp = "http://www.imsproject.org/xsd/imscp_rootv1p1p2";
            XNamespace adlcp = "http://www.adlnet.org/xsd/adlcp_rootv1p2";

            // Get all resources for reference lookup
            var resourceElements = document
                .Element(imscp + "manifest")?
                .Element(imscp + "resources")?
                .Elements(imscp + "resource")
                .ToDictionary(r => r.Attribute("identifier")?.Value, r => r.Attribute("href")?.Value);

            // Navigate through the organization structure
            var organization = document
                .Element(imscp + "manifest")?
                .Element(imscp + "organizations")?
                .Element(imscp + "organization");

            if (organization != null)
            {
                ExtractLaunchPagesFromItems(organization.Elements(imscp + "item"), launchPages, resourceElements, imscp, 0, null);
            }

            return launchPages;
        }

        private void ExtractLaunchPagesFromItems(IEnumerable<XElement> items, List<ScormLaunchPage> launchPages,
            Dictionary<string, string> resourceElements, XNamespace imscp, int level, string parentIdentifier)
        {
            foreach (var item in items)
            {
                var identifier = item.Attribute("identifier")?.Value;
                var identifierRef = item.Attribute("identifierref")?.Value;
                var title = item.Element(imscp + "title")?.Value;
                var parameters = item.Attribute("parameters")?.Value;

                // If this item has an identifierref, it's a launchable SCO/asset
                if (!string.IsNullOrEmpty(identifierRef) && resourceElements.ContainsKey(identifierRef))
                {
                    var launchPage = new ScormLaunchPage
                    {
                        Identifier = identifier,
                        ResourceIdentifier = identifierRef,
                        Href = resourceElements[identifierRef],
                        Title = title ?? identifier,
                        Parameters = parameters,
                        Level = level,
                        ParentIdentifier = parentIdentifier
                    };

                    launchPages.Add(launchPage);
                }

                // Process child items (both for launchable items and aggregation items)
                var childItems = item.Elements(imscp + "item");
                if (childItems.Any())
                {
                    var currentParent = !string.IsNullOrEmpty(identifierRef) ? identifier : parentIdentifier;
                    ExtractLaunchPagesFromItems(childItems, launchPages, resourceElements, imscp, level + 1, currentParent);
                }
            }
        }

        public List<ScormLaunchPage> GetLaunchPagesFromFile(string filePath)
        {
            var xmlContent = System.IO.File.ReadAllText(filePath);
            return GetLaunchPages(xmlContent);
        }

        public List<ScormLaunchPage> GetLaunchPagesHierarchical(string xmlContent)
        {
            var document = XDocument.Parse(xmlContent);
            var rootPages = new List<ScormLaunchPage>();

            // Define namespaces used in the manifest
            XNamespace imscp = "http://www.imsproject.org/xsd/imscp_rootv1p1p2";
            XNamespace adlcp = "http://www.adlnet.org/xsd/adlcp_rootv1p2";

            // Get all resources for reference lookup
            var resourceElements = document
                .Element(imscp + "manifest")?
                .Element(imscp + "resources")?
                .Elements(imscp + "resource")
                .ToDictionary(r => r.Attribute("identifier")?.Value, r => r.Attribute("href")?.Value);

            // Navigate through the organization structure
            var organization = document
                .Element(imscp + "manifest")?
                .Element(imscp + "organizations")?
                .Element(imscp + "organization");

            if (organization != null)
            {
                BuildHierarchicalLaunchPages(organization.Elements(imscp + "item"), rootPages, resourceElements, imscp, 0);
            }

            return rootPages;
        }

        private void BuildHierarchicalLaunchPages(IEnumerable<XElement> items, List<ScormLaunchPage> parentList,
            Dictionary<string, string> resourceElements, XNamespace imscp, int level)
        {
            foreach (var item in items)
            {
                var identifier = item.Attribute("identifier")?.Value;
                var identifierRef = item.Attribute("identifierref")?.Value;
                var title = item.Element(imscp + "title")?.Value;
                var parameters = item.Attribute("parameters")?.Value;

                ScormLaunchPage currentPage = null;

                // If this item has an identifierref, it's a launchable SCO/asset
                if (!string.IsNullOrEmpty(identifierRef) && resourceElements.ContainsKey(identifierRef))
                {
                    currentPage = new ScormLaunchPage
                    {
                        Identifier = identifier,
                        ResourceIdentifier = identifierRef,
                        Href = resourceElements[identifierRef],
                        Title = title ?? identifier,
                        Parameters = parameters,
                        Level = level
                    };

                    parentList.Add(currentPage);
                }
                // If it's an aggregation item (no identifierref but has title), create a container
                else if (!string.IsNullOrEmpty(title))
                {
                    currentPage = new ScormLaunchPage
                    {
                        Identifier = identifier,
                        Title = title,
                        Level = level,
                        Href = null // Aggregation items are not directly launchable
                    };

                    parentList.Add(currentPage);
                }

                // Process child items
                var childItems = item.Elements(imscp + "item");
                if (childItems.Any() && currentPage != null)
                {
                    BuildHierarchicalLaunchPages(childItems, currentPage.Children, resourceElements, imscp, level + 1);
                }
            }
        }

        public string GenerateJavaScriptResourceList(List<ScormResource> resources, string variableName = "scormResources")
        {
            var jsBuilder = new System.Text.StringBuilder();

            jsBuilder.AppendLine($"const {variableName} = [");

            for (int i = 0; i < resources.Count; i++)
            {
                var resource = resources[i];
                jsBuilder.AppendLine("  {");
                jsBuilder.AppendLine($"    identifier: \"{EscapeJavaScript(resource.Identifier)}\",");
                jsBuilder.AppendLine($"    href: \"{EscapeJavaScript(resource.Href)}\",");
                jsBuilder.AppendLine($"    title: \"{EscapeJavaScript(resource.Title)}\"");

                if (i < resources.Count - 1)
                    jsBuilder.AppendLine("  },");
                else
                    jsBuilder.AppendLine("  }");
            }

            jsBuilder.AppendLine("];");

            return jsBuilder.ToString();
        }

        public string GenerateJavaScriptWithFunctions(List<ScormResource> resources, string variableName = "scormResources")
        {
            var jsBuilder = new System.Text.StringBuilder();

            // Generate the resource array
            jsBuilder.AppendLine(GenerateJavaScriptResourceList(resources, variableName));
            jsBuilder.AppendLine();

            // Add utility functions
            jsBuilder.AppendLine("// Utility functions for working with SCORM resources");
            jsBuilder.AppendLine($"function getResourceById(id) {{");
            jsBuilder.AppendLine($"  return {variableName}.find(resource => resource.identifier === id);");
            jsBuilder.AppendLine("}");
            jsBuilder.AppendLine();

            jsBuilder.AppendLine($"function getResourceByTitle(title) {{");
            jsBuilder.AppendLine($"  return {variableName}.find(resource => resource.title === title);");
            jsBuilder.AppendLine("}");
            jsBuilder.AppendLine();

            jsBuilder.AppendLine($"function getAllResourceTitles() {{");
            jsBuilder.AppendLine($"  return {variableName}.map(resource => resource.title);");
            jsBuilder.AppendLine("}");
            jsBuilder.AppendLine();

            jsBuilder.AppendLine($"function createResourceListHTML() {{");
            jsBuilder.AppendLine($"  let html = '<ul class=\"scorm-resource-list\">';");
            jsBuilder.AppendLine($"  {variableName}.forEach(resource => {{");
            jsBuilder.AppendLine($"    html += `<li><a href=\"${{resource.href}}\" data-id=\"${{resource.identifier}}\">${{resource.title}}</a></li>`;");
            jsBuilder.AppendLine($"  }});");
            jsBuilder.AppendLine($"  html += '</ul>';");
            jsBuilder.AppendLine($"  return html;");
            jsBuilder.AppendLine("}");

            return jsBuilder.ToString();
        }

        private string EscapeJavaScript(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return input
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("'", "\\'")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
        }

        public void WriteJavaScriptToFile(List<ScormResource> resources, string filePath, string variableName = "scormResources", bool includeFunctions = true)
        {
            string jsContent = includeFunctions
                ? GenerateJavaScriptWithFunctions(resources, variableName)
                : GenerateJavaScriptResourceList(resources, variableName);

            System.IO.File.WriteAllText(filePath, jsContent);
        }

        public string GenerateJavaScriptLaunchPages(List<ScormLaunchPage> launchPages, string variableName = "scormLaunchPages")
        {
            var jsBuilder = new System.Text.StringBuilder();

            jsBuilder.AppendLine($"const {variableName} = [");

            for (int i = 0; i < launchPages.Count; i++)
            {
                var page = launchPages[i];
                jsBuilder.AppendLine("  {");
                jsBuilder.AppendLine($"    identifier: \"{EscapeJavaScript(page.Identifier)}\",");
                jsBuilder.AppendLine($"    resourceIdentifier: \"{EscapeJavaScript(page.ResourceIdentifier)}\",");
                jsBuilder.AppendLine($"    href: \"{EscapeJavaScript(page.Href)}\",");
                jsBuilder.AppendLine($"    title: \"{EscapeJavaScript(page.Title)}\",");
                jsBuilder.AppendLine($"    parameters: \"{EscapeJavaScript(page.Parameters)}\",");
                jsBuilder.AppendLine($"    level: {page.Level},");
                jsBuilder.AppendLine($"    parentIdentifier: \"{EscapeJavaScript(page.ParentIdentifier)}\"");

                if (i < launchPages.Count - 1)
                    jsBuilder.AppendLine("  },");
                else
                    jsBuilder.AppendLine("  }");
            }

            jsBuilder.AppendLine("];");
            jsBuilder.AppendLine();

            // Add utility functions for launch pages
            jsBuilder.AppendLine("// Utility functions for SCORM launch pages");
            jsBuilder.AppendLine($"function getLaunchPageById(id) {{");
            jsBuilder.AppendLine($"  return {variableName}.find(page => page.identifier === id);");
            jsBuilder.AppendLine("}");
            jsBuilder.AppendLine();

            jsBuilder.AppendLine($"function getLaunchablePages() {{");
            jsBuilder.AppendLine($"  return {variableName}.filter(page => page.href && page.href !== '');");
            jsBuilder.AppendLine("}");
            jsBuilder.AppendLine();

            jsBuilder.AppendLine($"function getPagesByLevel(level) {{");
            jsBuilder.AppendLine($"  return {variableName}.filter(page => page.level === level);");
            jsBuilder.AppendLine("}");
            jsBuilder.AppendLine();

            jsBuilder.AppendLine($"function buildLaunchUrl(page) {{");
            jsBuilder.AppendLine($"  if (!page.href) return null;");
            jsBuilder.AppendLine($"  return page.parameters ? page.href + page.parameters : page.href;");
            jsBuilder.AppendLine("}");

            return jsBuilder.ToString();
        }
    }

    // Example usage
    public class Program
    {
        public static void Main()
        {
            var parser = new ScormManifestParser();

            // Example usage with file
            // var resources = parser.ParseManifestFromFile("path/to/imsmanifest.xml");

            // Example usage with XML content
            string xmlContent = @"<?xml version=""1.0"" standalone=""no"" ?>
            <!-- Your SCORM manifest XML content here -->";

            var resources = parser.ParseManifest(xmlContent);

            // Display parsed resources
            foreach (var resource in resources)
            {
                Console.WriteLine($"ID: {resource.Identifier}");
                Console.WriteLine($"Href: {resource.Href}");
                Console.WriteLine($"Title: {resource.Title}");
                Console.WriteLine("---");
            }

            // Get launch pages
            var launchPages = parser.GetLaunchPages(xmlContent);
            Console.WriteLine("\nLaunch Pages:");
            foreach (var page in launchPages)
            {
                var indent = new string(' ', page.Level * 2);
                Console.WriteLine($"{indent}- {page.Title} ({page.Href}){(string.IsNullOrEmpty(page.Parameters) ? "" : " " + page.Parameters)}");
            }

            // Get hierarchical launch pages
            var hierarchicalPages = parser.GetLaunchPagesHierarchical(xmlContent);
            Console.WriteLine("\nHierarchical Launch Pages:");
            DisplayHierarchicalPages(hierarchicalPages, 0);

            // Generate JavaScript
            Console.WriteLine("\nGenerated JavaScript for Resources:");
            Console.WriteLine(parser.GenerateJavaScriptWithFunctions(resources));

            Console.WriteLine("\nGenerated JavaScript for Launch Pages:");
            Console.WriteLine(parser.GenerateJavaScriptLaunchPages(launchPages));

            // Or write to file
            // parser.WriteJavaScriptToFile(resources, "scorm-resources.js");
        }

        private static void DisplayHierarchicalPages(List<ScormLaunchPage> pages, int level)
        {
            foreach (var page in pages)
            {
                var indent = new string(' ', level * 2);
                var launchable = !string.IsNullOrEmpty(page.Href) ? $" -> {page.Href}" : " [Section]";
                Console.WriteLine($"{indent}- {page.Title}{launchable}");

                if (page.Children.Any())
                {
                    DisplayHierarchicalPages(page.Children, level + 1);
                }
            }
        }
    }
}