using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PocAspNetCoreBlazorAntDesign.Endpoints;
using PocAspNetCoreBlazorAntDesign.Services.CodeServce;
using PocAspNetCoreBlazorAntDesign.Services;

namespace PocAspNetCoreBlazorAntDesign.Controllers
{
    public class TokenEndpointController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthorizeResultService _authorizeResultService;
        public TokenEndpointController(IHttpContextAccessor httpContextAccessor, IAuthorizeResultService authorizeResultService)
        {
            _httpContextAccessor = httpContextAccessor;
            _authorizeResultService = authorizeResultService;
        }

        [HttpPost("~/auth/realms/master/token")]
        public JsonResult Token()
        {
            var result = _authorizeResultService.GenerateToken(_httpContextAccessor);

            if (result.HasError)
                return Json("0");

            return Json(result);
        }
    }
}
