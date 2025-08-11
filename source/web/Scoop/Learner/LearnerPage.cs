using System;
using System.Web;

namespace Scoop
{
    public class LearnerPage : System.Web.UI.Page
    {
        protected string CurrentLearnerEmail
            => IsUserAuthenticated() ? Global.GetCurrentUser().UserEmail : CreateAnonymousEmail();

        protected Guid CurrentLearnerId
            => IsUserAuthenticated() ? Global.GetCurrentUser().UserIdentifier.Value : Guid.Empty;

        protected string CurrentOrganizationSlug
            => AppSettings.OrganizationSlug;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (!IsUserAuthenticated())
            {
                var loginUrl = Navigator.LoginUrl;

                var returnUrl = Request.Url.AbsoluteUri;

                var url = string.Format("{0}?ReturnUrl={1}", loginUrl, HttpUtility.UrlEncode(returnUrl));

                Response.Redirect(url);
            }

            if (!IsUserAuthorized())
            {
                Response.Redirect("~/Error/Deny.aspx");
            }
        }

        private string CreateAnonymousEmail()
        {
            var domain = UrlHelper.GetDomainWithoutSubdomain(Request.Url.Host);
            return "someone@" + domain;
        }

        protected virtual bool IsUserAuthenticated()
        {
            var user = Global.GetCurrentUser();

            return user != null && user.IsAuthenticated;
        }

        protected virtual bool IsUserAuthorized()
        {
            return true;
        }
    }
}