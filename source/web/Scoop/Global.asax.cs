using System;
using System.Web;

namespace Scoop
{
    public class Global : HttpApplication
    {
        static Global()
        {
            ByteHexMapping = new uint[256];
            for (var i = 0; i < ByteHexMapping.Length; i++)
            {
                var s = i.ToString("X2");
                ByteHexMapping[i] = ((uint)s[0]) + ((uint)s[1] << 16);
            }
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            Navigator.RepoUrl = "https://github.com/daniel-miller/scoop";
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (!Navigator.IsInitialized)
                Navigator.Initialize(HttpContext.Current);
        }

        private void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            var context = HttpContext.Current;
            if (context.User != null)
                return;

            var token = GetToken(context.Request);
            if (token != null && !token.IsExpired() && token.UserEmail != null)
                context.User = token;
        }

        void Session_Start(object sender, EventArgs e)
        {
            var token = Session["User"] as CookieToken;

            if (token != null)
                return;

            token = Context.User as CookieToken;

            if (token != null && !token.IsExpired() && token.UserEmail != null)
            {
                var writer = new LearnerWriter();

                writer.Register(token.UserIdentifier.Value, token.UserEmail);

                Session["User"] = token;
            }
        }

        public static CookieToken GetToken(HttpRequest request)
        {
            var cookieValue = request.Cookies.Get(AppSettings.CookieName)?.Value;

            var token = GetTokenEncoder().Deserialize(cookieValue);

            if (!IsValid(token))
                return null;

            token.CurrentBrowser = request.Browser.Browser;
            token.CurrentBrowserVersion = request.Browser.Version;

            return token;
        }

        public static CookieToken GetCurrentUser()
            => HttpContext.Current.User as CookieToken;

        public static string GetCurrentOrganization()
            => (HttpContext.Current.User as CookieToken)?.OrganizationCode ?? "demo";

        #region Methods (token validation)

        private static CookieTokenEncoder _tokenEncoder;

        private static CookieTokenEncoder GetTokenEncoder()
        {
            if (_tokenEncoder == null)
                _tokenEncoder = new CookieTokenEncoder(
                    AppSettings.IsCookieEncrypted,
                    AppSettings.CookieSecret,
                    true);

            return _tokenEncoder;
        }

        private static string CreateValidationKey(CookieToken token)
        {
            var secret = AppSettings.CookieSecret;
            string text = $"{token.UserEmail}/{token.ImpersonatorOrganization}/{token.ImpersonatorUser}/{secret}/{token.ID}";
            return HashText(text);
        }

        private static string HashText(string text)
        {
            var hash = EncryptionHelper.ComputeHashSha256(text);
            return ByteArrayToHex(hash);
        }

        private static bool IsValid(CookieToken token)
        {
            return token != null && token.ValidationKey == CreateValidationKey(token);
        }

        public static string ByteArrayToHex(byte[] value)
        {
            var result = new char[value.Length * 2];
            for (int i = 0, j = 0; i < value.Length; i++, j += 2)
                ByteToHex(result, j, value[i]);
            return new string(result);
        }

        private static readonly uint[] ByteHexMapping;

        private static void ByteToHex(char[] array, int offset, byte value)
        {
            var mapping = ByteHexMapping[value];
            array[offset] = (char)mapping;
            array[offset + 1] = (char)(mapping >> 16);
        }

        #endregion

    }
}