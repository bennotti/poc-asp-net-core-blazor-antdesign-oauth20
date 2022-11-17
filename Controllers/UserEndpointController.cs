using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PocAspNetCoreBlazorAntDesign.Endpoints;

namespace PocAspNetCoreBlazorAntDesign.Controllers
{
    public class UserEndpointController : Controller
    {
        [HttpGet("~/auth/realms/master/user/info")]
        public JsonResult UserInfo()
        {
            return Json(new
            {
                sub = "856933325856"
            });
        }
    }
}
