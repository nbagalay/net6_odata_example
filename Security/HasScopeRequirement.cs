using Microsoft.AspNetCore.Authorization;

namespace OData_webapi_netcore6.Security
{
    public class HasScopeRequirement : IAuthorizationRequirement
    {
        public HasScopeRequirement(string scope, string issuer)
        {
            this.Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.Issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));
        }

        public string Issuer { get; }

        public string Scope { get; }
    }
}
