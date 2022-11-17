﻿using Microsoft.IdentityModel.Tokens;
using PocAspNetCoreBlazorAntDesign.Common;
using PocAspNetCoreBlazorAntDesign.Models;
using PocAspNetCoreBlazorAntDesign.OauthRequest;
using PocAspNetCoreBlazorAntDesign.OauthResponse;
using PocAspNetCoreBlazorAntDesign.Services.CodeServce;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PocAspNetCoreBlazorAntDesign.Services
{
    public class AuthorizeResultService : IAuthorizeResultService
    {
        private static string keyAlg = "66007d41-6924-49f2-ac0c-e63c4b1a1730";
        private readonly ClientStore _clientStore = new ClientStore();
        private readonly ICodeStoreService _codeStoreService;
        public AuthorizeResultService(ICodeStoreService codeStoreService)
        {
            _codeStoreService = codeStoreService;
        }
        public AuthorizeResponse AuthorizeRequest(IHttpContextAccessor httpContextAccessor, AuthorizationRequest authorizationRequest)
        {
            AuthorizeResponse response = new AuthorizeResponse();

            if (httpContextAccessor == null)
            {
                response.Error = ErrorTypeEnum.ServerError.GetEnumDescription();
                return response;
            }

            var client = VerifyClientById(authorizationRequest.client_id);
            if (!client.IsSuccess)
            {
                response.Error = client.ErrorDescription;
                return response;
            }

            if (string.IsNullOrEmpty(authorizationRequest.response_type) || authorizationRequest.response_type != "code")
            {
                response.Error = ErrorTypeEnum.InvalidRequest.GetEnumDescription();
                response.ErrorDescription = "response_type is required or is not valid";
                return response;
            }

            if (!authorizationRequest.redirect_uri.IsRedirectUriStartWithHttps() && !httpContextAccessor.HttpContext.Request.IsHttps)
            {
                response.Error = ErrorTypeEnum.InvalidRequest.GetEnumDescription();
                response.ErrorDescription = "redirect_url is not secure, MUST be TLS";
                return response;
            }


            // check the return url is match the one that in the client store


            // check the scope in the client store with the
            // one that is comming from the request MUST be matched at leaset one

            var scopes = authorizationRequest.scope.Split(' ');

            var clientScopes = from m in client.Client.AllowedScopes
                               where scopes.Contains(m)
                               select m;

            if (!clientScopes.Any())
            {
                response.Error = ErrorTypeEnum.InValidScope.GetEnumDescription();
                response.ErrorDescription = "scopes are invalids";
                return response;
            }

            string nonce = httpContextAccessor.HttpContext.Request.Query["nonce"].ToString();

            // Verify that a scope parameter is present and contains the openid scope value.
            // (If no openid scope value is present,
            // the request may still be a valid OAuth 2.0 request, but is not an OpenID Connect request.)

            string code = _codeStoreService.GenerateAuthorizationCode(authorizationRequest.client_id, clientScopes.ToList());
            if (code == null)
            {
                response.Error = ErrorTypeEnum.TemporarilyUnAvailable.GetEnumDescription();
                return response;
            }

            response.RedirectUri = client.Client.RedirectUri + "?response_type=code" + "&state=" + authorizationRequest.state;
            response.Code = code;
            response.State = authorizationRequest.state;
            response.RequestedScopes = clientScopes.ToList();
            response.Nonce = nonce;

            return response;

        }




        private CheckClientResult VerifyClientById(string clientId, bool checkWithSecret = false, string clientSecret = null)
        {
            CheckClientResult result = new CheckClientResult() { IsSuccess = false };

            if (!string.IsNullOrWhiteSpace(clientId))
            {
                var client = _clientStore.Clients.Where(x => x.ClientId.Equals(clientId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                if (client != null)
                {
                    if (checkWithSecret && !string.IsNullOrEmpty(clientSecret))
                    {
                        bool hasSamesecretId = client.ClientSecret.Equals(clientSecret, StringComparison.InvariantCulture);
                        if (!hasSamesecretId)
                        {
                            result.Error = ErrorTypeEnum.InvalidClient.GetEnumDescription();
                            return result;
                        }
                    }


                    // check if client is enabled or not

                    if (client.IsActive)
                    {
                        result.IsSuccess = true;
                        result.Client = client;

                        return result;
                    }
                    else
                    {
                        result.ErrorDescription = ErrorTypeEnum.UnAuthoriazedClient.GetEnumDescription();
                        return result;
                    }
                }
            }

            result.ErrorDescription = ErrorTypeEnum.AccessDenied.GetEnumDescription();
            return result;
        }

        public TokenResponse GenerateToken(IHttpContextAccessor httpContextAccessor)
        {
            TokenRequest request = new TokenRequest();

            request.CodeVerifier = httpContextAccessor.HttpContext.Request.Form["code_verifier"];
            request.ClientId = httpContextAccessor.HttpContext.Request.Form["client_id"];
            request.ClientSecret = httpContextAccessor.HttpContext.Request.Form["client_secret"];
            request.Code = httpContextAccessor.HttpContext.Request.Form["code"];
            request.GrantType = httpContextAccessor.HttpContext.Request.Form["grant_type"];
            request.RedirectUri = httpContextAccessor.HttpContext.Request.Form["redirect_uri"];

            var checkClientResult = this.VerifyClientById(request.ClientId, true, request.ClientSecret);
            if (!checkClientResult.IsSuccess)
            {
                return new TokenResponse { Error = checkClientResult.Error, ErrorDescription = checkClientResult.ErrorDescription };
            }

            // check code from the Concurrent Dictionary
            var clientCodeChecker = _codeStoreService.GetClientDataByCode(request.Code);
            if (clientCodeChecker == null)
                return new TokenResponse { Error = ErrorTypeEnum.InvalidGrant.GetEnumDescription() };


            // check if the current client who is one made this authentication request

            if (request.ClientId != clientCodeChecker.ClientId)
                return new TokenResponse { Error = ErrorTypeEnum.InvalidGrant.GetEnumDescription() };

            // TODO: 
            // also I have to check the rediret uri 


            // Now here I will Issue the Id_token

            JwtSecurityToken id_token = null;
            if (clientCodeChecker.IsOpenId)
            {
                // Generate Identity Token

                int iat = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;


                string[] amrs = new string[] { "pwd" };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyAlg));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>()
                {
                    new Claim("sub", "856933325856"),
                    new Claim("given_name", "Mohammed Ahmed Hussien"),
                    new Claim("iat", iat.ToString(), ClaimValueTypes.Integer), // time stamp
                    //new Claim("nonce", clientCodeChecker.Nonce)
                };
                foreach (var amr in amrs)
                    claims.Add(new Claim("amr", amr));// authentication method reference 

                id_token = new JwtSecurityToken("https://localhost:7261", request.ClientId, claims, signingCredentials: credentials,
                    expires: DateTime.UtcNow.AddMinutes(
                       int.Parse("5")));

            }

            // Here I have to generate access token 
            var key_at = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyAlg));
            var credentials_at = new SigningCredentials(key_at, SecurityAlgorithms.HmacSha256);

            var claims_at = new List<Claim>();


            var access_token = new JwtSecurityToken("https://localhost:7261", request.ClientId, claims_at, signingCredentials: credentials_at,
                expires: DateTime.UtcNow.AddMinutes(
                   int.Parse("5")));

            // here remoce the code from the Concurrent Dictionary
            _codeStoreService.RemoveClientDataByCode(request.Code);

            return new TokenResponse
            {
                access_token = new JwtSecurityTokenHandler().WriteToken(access_token),
                id_token = id_token != null ? new JwtSecurityTokenHandler().WriteToken(id_token) : null,
                code = request.Code
            };
        }
    }
}
