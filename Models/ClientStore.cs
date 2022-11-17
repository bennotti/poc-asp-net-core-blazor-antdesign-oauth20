namespace PocAspNetCoreBlazorAntDesign.Models
{
    public class ClientStore
    {
        public IEnumerable<Client> Clients = new[]
        {
            new Client
            {
                ClientName = "teste",
                ClientId = "1",
                ClientSecret = "123456789",
                AllowedScopes = new[]{ "openid", "profile"},
                GrantType = GrantTypes.Code,
                IsActive = true,
                ClientUri = "http://localhost:3000",
                RedirectUri = "http://localhost:3000/response"
            }
        };
    }
}
