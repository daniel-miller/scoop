using System;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace Scoop
{
    public static class UrlHelper
    {
        /// <summary>
        /// Gets the first logical path segment after the application root.
        /// Handles nested IIS applications by excluding the application path.
        /// </summary>
        /// <param name="rawUrl">The raw URL from the request</param>
        /// <returns>First path segment relative to the application, or null if none</returns>
        public static string GetFirstPathSegment(string rawUrl)
        {
            if (string.IsNullOrWhiteSpace(rawUrl))
                return null;

            // Remove query string and fragment
            var pathOnly = rawUrl.Split(new[] { '?', '#' }, 2)[0];

            // Get the application path (e.g., "/MyApp/SubApp" for nested apps)
            var appPath = GetApplicationPath();

            // Remove the application path if present
            if (!string.IsNullOrEmpty(appPath) && appPath != "/" &&
                pathOnly.StartsWith(appPath, StringComparison.OrdinalIgnoreCase))
            {
                pathOnly = pathOnly.Substring(appPath.Length);
            }

            // Ensure we have a leading slash for consistency in parsing
            if (!pathOnly.StartsWith("/"))
                pathOnly = "/" + pathOnly;

            // Remove the leading slash and split
            pathOnly = pathOnly.TrimStart('/');

            if (string.IsNullOrEmpty(pathOnly))
                return null;

            // Get the first segment
            var firstSegment = pathOnly.Split(new[] { '/' }, 2)[0];

            // Return null for empty segments, otherwise return the segment
            return string.IsNullOrWhiteSpace(firstSegment) ? null : firstSegment;
        }

        /// <summary>
        /// Gets the application path, preferring HttpContext when available,
        /// falling back to HostingEnvironment for non-request contexts
        /// </summary>
        private static string GetApplicationPath()
        {
            // Try to get from current HttpContext first (most accurate during requests)
            if (HttpContext.Current?.Request != null)
            {
                return HttpContext.Current.Request.ApplicationPath;
            }

            // Fallback to HostingEnvironment (works outside of request context)
            var appPath = HostingEnvironment.ApplicationVirtualPath;
            return string.IsNullOrEmpty(appPath) ? "/" : appPath;
        }

        /// <summary>
        /// Alternative overload that accepts HttpRequest for testing or when context is explicit
        /// </summary>
        public static string GetFirstPathSegment(string rawUrl, HttpRequest request)
        {
            if (string.IsNullOrWhiteSpace(rawUrl))
                return null;

            var pathOnly = rawUrl.Split(new[] { '?', '#' }, 2)[0];
            var appPath = request?.ApplicationPath ?? "/";

            if (!string.IsNullOrEmpty(appPath) && appPath != "/" &&
                pathOnly.StartsWith(appPath, StringComparison.OrdinalIgnoreCase))
            {
                pathOnly = pathOnly.Substring(appPath.Length);
            }

            if (!pathOnly.StartsWith("/"))
                pathOnly = "/" + pathOnly;

            pathOnly = pathOnly.TrimStart('/');

            if (string.IsNullOrEmpty(pathOnly))
                return null;

            var firstSegment = pathOnly.Split(new[] { '/' }, 2)[0];
            return string.IsNullOrWhiteSpace(firstSegment) ? null : firstSegment;
        }

        /// <summary>
        /// Overload for testing with explicit application path
        /// </summary>
        public static string GetFirstPathSegment(string rawUrl, string applicationPath)
        {
            if (string.IsNullOrWhiteSpace(rawUrl))
                return null;

            var pathOnly = rawUrl.Split(new[] { '?', '#' }, 2)[0];

            if (!string.IsNullOrEmpty(applicationPath) && applicationPath != "/" &&
                pathOnly.StartsWith(applicationPath, StringComparison.OrdinalIgnoreCase))
            {
                pathOnly = pathOnly.Substring(applicationPath.Length);
            }

            if (!pathOnly.StartsWith("/"))
                pathOnly = "/" + pathOnly;

            pathOnly = pathOnly.TrimStart('/');

            if (string.IsNullOrEmpty(pathOnly))
                return null;

            var firstSegment = pathOnly.Split(new[] { '/' }, 2)[0];
            return string.IsNullOrWhiteSpace(firstSegment) ? null : firstSegment;
        }

        /// <summary>
        /// Extracts the domain portion without scheme and subdomain
        /// </summary>
        /// <param name="url">The URL to extract domain from</param>
        /// <returns>The domain without scheme and subdomain (e.g., "example.com")</returns>
        public static string GetDomainWithoutSubdomain(Uri url)
        {
            if (url == null)
                return null;

            return GetDomainWithoutSubdomain(url.Host);
        }

        /// <summary>
        /// Extracts the domain portion without subdomain from a host string
        /// </summary>
        /// <param name="host">The host string (e.g., "api.subdomain.example.com")</param>
        /// <returns>The domain without subdomain (e.g., "example.com")</returns>
        public static string GetDomainWithoutSubdomain(string host)
        {
            if (string.IsNullOrEmpty(host))
                return null;

            // Split host by dots
            var parts = host.Split('.');

            // Handle IP addresses (return as-is)
            if (IsIpAddress(host))
                return host;

            // Handle localhost
            if (host.Equals("localhost", StringComparison.OrdinalIgnoreCase))
                return host;

            // Need at least 2 parts for a valid domain (domain.tld)
            if (parts.Length < 2)
                return host;

            // Handle common TLD patterns
            if (parts.Length >= 2)
            {
                var lastPart = parts[parts.Length - 1];
                var secondLastPart = parts[parts.Length - 2];

                // Handle common two-part TLDs like .co.uk, .com.au, etc.
                if (IsCommonTwoPartTld(secondLastPart, lastPart) && parts.Length >= 3)
                {
                    // Return domain.co.uk format
                    return string.Join(".", parts.Skip(parts.Length - 3).Take(3));
                }
                else
                {
                    // Return domain.com format
                    return string.Join(".", parts.Skip(parts.Length - 2).Take(2));
                }
            }

            return host;
        }

        private static bool IsIpAddress(string host)
        {
            return System.Net.IPAddress.TryParse(host, out _);
        }

        private static bool IsCommonTwoPartTld(string secondLast, string last)
        {
            var twoPartTlds = new[]
            {
                "co.uk", "com.au", "co.nz", "co.za", "com.br", "co.jp",
                "com.mx", "co.in", "com.sg", "co.kr", "com.tr", "co.il"
            };

            var combined = $"{secondLast}.{last}";
            return twoPartTlds.Contains(combined, StringComparer.OrdinalIgnoreCase);
        }
    }
}