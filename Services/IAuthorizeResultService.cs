using PocAspNetCoreBlazorAntDesign.OauthRequest;
using PocAspNetCoreBlazorAntDesign.OauthResponse;

namespace PocAspNetCoreBlazorAntDesign.Services
{
    public interface IAuthorizeResultService
    {
        AuthorizeResponse AuthorizeRequest(IHttpContextAccessor httpContextAccessor, AuthorizationRequest authorizationRequest);
        TokenResponse GenerateToken(IHttpContextAccessor httpContextAccessor);
    }
}
