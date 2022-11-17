using System.Security.Claims;

namespace PocAspNetCoreBlazorAntDesign.Models
{
    public class AuthorizationCode
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }

        public DateTime CreationTime { get; set; } = DateTime.UtcNow;


        public bool IsOpenId { get; set; }
        public IList<string> RequestedScopes { get; set; }

        public ClaimsPrincipal Subject { get; set; }
        public string Nonce { get; set; }

        public string SubjectId
        {
            get
            {
                var data = Subject.Identity as ClaimsIdentity;

                var claim = data.FindFirst("sub");
                if (claim != null)
                {
                    return claim.Value;
                }
                else
                {
                    return null;
                }

            }
        }
    }
}
