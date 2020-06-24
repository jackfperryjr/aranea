
using Microsoft.AspNetCore.Mvc;

namespace Aranea.Api.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class SwaggerController: ControllerBase
    {
        [HttpGet]
        [Route("")]
        [Route("swagger")]
        [Route("index.html")]
        public RedirectResult DefaultRedirect()
        {
            return Redirect("~/swagger/index.html");
        }
    }
}