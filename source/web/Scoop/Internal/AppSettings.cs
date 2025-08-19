using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Scoop
{
    public static class AppSettings
    {
        public static string ConnectionString { get; private set; }
        public static string CookieName { get; private set; }
        public static string CookieSecret { get; private set; }
        public static bool IsCookieEncrypted { get; private set; }
        public static string PackageFolder { get; private set; }
        public static string LoginUrl { get; private set; }
        public static string Whitelist { get; private set; }

        static AppSettings()
        {
            ConnectionString = ConfigurationManager.AppSettings["Scoop:ConnectionString"];
            CookieName = ConfigurationManager.AppSettings["Scoop:CookieName"];
            CookieSecret = ConfigurationManager.AppSettings["Scoop:CookieSecret"];
            IsCookieEncrypted = ConfigurationManager.AppSettings["Scoop:CookieEncryption"] == "Enabled";
            PackageFolder = ConfigurationManager.AppSettings["Scoop:PackageFolder"];
            LoginUrl = ConfigurationManager.AppSettings["Scoop:LoginUrl"];
            Whitelist = ConfigurationManager.AppSettings["Scoop:Whitelist"];
        }

        public static string GetPhysicalPath(string relativePath)
        {
            string physicalPath = HttpContext.Current.Server.MapPath($"~/{relativePath}");

            return physicalPath;
        }

        public static string GetPhysicalPathToPackage(string organization, string package)
        {
            var root = PackageFolder;

            string physicalPath = HttpContext.Current.Server.MapPath($"~/{root}/{organization}/{package}");

            return physicalPath;
        }

        public static string GetPhysicalPathToPackage(HttpContext context, string organizationSlug, string packageSlug)
        {
            var root = PackageFolder;

            string physicalPath = context.Server.MapPath($"~/{root}/{organizationSlug}/{packageSlug}");

            return physicalPath;
        }

        public static string OrganizationName
            => GetOrganizationName(HttpContext.Current);

        public static string OrganizationSlug
            => GetOrganizationSlug(HttpContext.Current);

        public static string GetOrganizationName(HttpContext context)
        {
            var organizationSlug = GetOrganizationSlug(context);

            switch (organizationSlug)
            {
                case "insite":
                    return "InSite";

                case "wolfmidstream":
                    return "Wolf Midstream";
            }

            var textInfo = new CultureInfo("en-US", false).TextInfo;

            return textInfo.ToTitleCase(organizationSlug);
        }

        public static string GetOrganizationSlug(HttpContext context)
        {
            var segment = UrlHelper.GetFirstPathSegment(context.Request.RawUrl);

            return segment;
        }

        public static bool IsWhitelisted(string ipAddress)
        {
            var items = Whitelist.Split(new[] { ',' });
            return items.Contains(ipAddress);
        }
    }
}