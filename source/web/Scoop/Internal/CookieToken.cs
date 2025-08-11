using System;
using System.Linq;
using System.Security.Principal;

namespace Scoop
{
    [Serializable]
    public class CookieToken : IPrincipal, IIdentity
    {
        #region Constants

        private const int EventDurationMinutes = 15;

        private const int DefaultLifetime = 60; // minutes

        private static TimeSpan _lifetime = TimeSpan.Zero;

        public static TimeSpan Lifetime()
        {
            if (_lifetime == TimeSpan.Zero)
                _lifetime = TimeSpan.FromMinutes(DefaultLifetime);

            return _lifetime;
        }

        public string CurrentOrganization { get; set; }
        public string CurrentBrowser { get; set; }
        public string CurrentBrowserVersion { get; set; }

        #endregion

        #region Properties (data)

        public string Session { get; set; }

        public Guid? OrganizationIdentifier { get; set; }
        public string OrganizationCode { get; set; }

        public Guid? UserIdentifier { get; set; }
        public string UserEmail { get; set; }

        public bool IsAdministrator { get; set; }
        public bool IsDeveloper { get; set; }
        public bool IsOperator { get; set; }

        public Guid[] UserRoles { get; set; }

        public string ImpersonatorOrganization { get; set; }

        public string ImpersonatorUser { get; set; }

        public string Language { get; set; }

        public string TimeZoneId { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public Guid ID { get; set; }

        public string ValidationKey { get; set; }

        public string AuthenticationSource { get; set; }

        #endregion

        public bool IsActive() => !IsExpired() && Modified.AddMinutes(EventDurationMinutes) > DateTime.UtcNow;

        public bool IsExpired() => Modified.Add(Lifetime()) <= DateTime.UtcNow;



        public IIdentity Identity => this;

        public string Name => UserEmail;

        public string AuthenticationType => AuthenticationSource;

        public bool IsAuthenticated => UserEmail != null;

        public bool IsInRole(string role)
        {
            if (UserRoles != null && Guid.TryParse(role, out Guid id))
                return UserRoles.Contains(id);

            return false;
        }



        public CookieToken()
        {
            ID = Guid.NewGuid();
            Created = DateTime.UtcNow;
            Modified = Created;
        }
    }
}