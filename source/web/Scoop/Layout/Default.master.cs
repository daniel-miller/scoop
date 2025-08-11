using System;

namespace Scoop
{
    public partial class DefaultMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            FaviconLink.Href = Navigator.FaviconPath;

            BrandLink.HRef = Navigator.HomePath;

            HomeLink.HRef = Navigator.HomePath;

            AboutLink.HRef = Navigator.AboutPath;

            ContactLink.HRef = Navigator.RepoUrl;

            GetStartedLink.HRef = Navigator.RepoUrl;

            PrivacyLink.HRef = Navigator.PrivacyPath;

            TermsLink.HRef = Navigator.TermsPath;

            ListLink.Visible = false;
            AdminLink.Visible = false;

            if (!Request.IsAuthenticated)
                return;

            ListLink.HRef = Navigator.ListPath;
            ListLink.Visible = true;

            if (!AppSettings.IsWhitelisted(Request.UserHostAddress))
                return;

            AdminLink.HRef = Navigator.AdminPath;
            AdminLink.Visible = true;
        }
    }
}