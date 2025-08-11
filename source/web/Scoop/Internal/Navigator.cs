using System;
using System.Web;

namespace Scoop
{
    // <summary>
    /// Provides centralized navigation paths for the application
    /// </summary>
    public static class Navigator
    {
        private static readonly object _lock = new object();

        private static bool _initialized = false;

        /// <summary>
        /// Gets the application root URL
        /// </summary>
        public static string AppRoot { get; private set; }

        /// <summary>
        /// Gets the About page path
        /// </summary>
        public static string AboutPath => AppRoot + "about";

        /// <summary>
        /// Gets the Privacy page path
        /// </summary>
        public static string PrivacyPath => AppRoot + "privacy";

        /// <summary>
        /// Gets the Terms page path
        /// </summary>
        public static string TermsPath => AppRoot + "terms";

        /// <summary>
        /// Gets or sets the repository URL
        /// </summary>
        public static string RepoUrl { get; set; }

        // Other common paths and URLs
        public static string AdminPath => AppRoot + "admin";
        public static string DemoUrl => MissingUrl;
        public static string FaviconPath => AppRoot + "favicon.ico";
        public static string HomePath => AppRoot;
        public static string ListPath => AppRoot + Global.GetCurrentOrganization();
        public static string LoginUrl => AppSettings.LoginUrl;
        public static string MoreUrl => MissingUrl;
        private static string MissingUrl => "#";

        /// <summary>
        /// Initializes the Navigator with the current HttpContext
        /// </summary>
        /// <param name="context">The current HttpContext</param>
        public static void Initialize(HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            lock (_lock)
            {
                AppRoot = context.Request.ApplicationPath;

                if (!AppRoot.EndsWith("/"))
                    AppRoot += "/";

                _initialized = true;
            }
        }

        /// <summary>
        /// Initializes the Navigator using the current HttpContext
        /// </summary>
        public static void Initialize()
        {
            Initialize(HttpContext.Current);
        }

        /// <summary>
        /// Gets whether the Navigator has been initialized
        /// </summary>
        public static bool IsInitialized => _initialized;

        /// <summary>
        /// Ensures the Navigator is initialized
        /// </summary>
        public static void EnsureInitialized()
        {
            if (!_initialized)
            {
                if (HttpContext.Current != null)
                    Initialize(HttpContext.Current);
                else
                    throw new InvalidOperationException("Navigator is not initialized and no HttpContext is available.");
            }
        }

        // Helper methods for building paths
        public static string GetPath(string relativePath)
        {
            EnsureInitialized();
            if (string.IsNullOrEmpty(relativePath))
                return AppRoot;

            if (relativePath.StartsWith("/"))
                relativePath = relativePath.Substring(1);

            return AppRoot + relativePath;
        }

        /// <summary>
        /// Gets the full URL for a relative path
        /// </summary>
        public static string GetFullUrl(string relativePath)
        {
            EnsureInitialized();
            var context = HttpContext.Current;
            if (context == null)
                return GetPath(relativePath);

            var request = context.Request;
            var baseUrl = $"{request.Url.Scheme}://{request.Url.Authority}";
            return baseUrl + GetPath(relativePath);
        }

        public static string ReportPath(string organization, string package)
            => AppRoot + $"{organization}/{package}/report";

        public static string LaunchPath(string organization, string package)
            => AppRoot + $"{organization}/{package}";
    }
}