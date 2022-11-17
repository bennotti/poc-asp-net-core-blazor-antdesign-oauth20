using PocAspNetCoreBlazorAntDesign.Models;

namespace PocAspNetCoreBlazorAntDesign.Services.CodeServce
{
    public interface ICodeStoreService
    {
        string GenerateAuthorizationCode(string clientId, IList<string> requestedScope);
        AuthorizationCode GetClientDataByCode(string key);
        AuthorizationCode UpdatedClientDataByCode(string key, IList<string> requestdScopes, string userName, string password = null, string nonce = null);
        AuthorizationCode RemoveClientDataByCode(string key);
    }
}
