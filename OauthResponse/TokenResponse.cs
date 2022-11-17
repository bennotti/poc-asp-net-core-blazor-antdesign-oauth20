using PocAspNetCoreBlazorAntDesign.Common;
using PocAspNetCoreBlazorAntDesign.Models;

namespace PocAspNetCoreBlazorAntDesign.OauthResponse
{
    public class TokenResponse
    {
        /// <summary>
        /// Oauth 2
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// OpenId Connect
        /// </summary>
        public string id_token { get; set; }

        /// <summary>
        /// By default is Bearer
        /// </summary>

        public string token_type { get; set; } = TokenTypeEnum.Bearer.GetEnumDescription();

        /// <summary>
        /// Authorization Code. This is always returned when using the Hybrid Flow.
        /// </summary>
        public string code { get; set; }



        // For Error Details if any

        public string Error { get; set; } = string.Empty;
        public string ErrorUri { get; set; }
        public string ErrorDescription { get; set; }
        public bool HasError => !string.IsNullOrEmpty(Error);
    }
}
