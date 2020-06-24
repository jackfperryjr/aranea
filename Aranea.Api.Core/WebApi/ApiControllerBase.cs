using Microsoft.AspNetCore.Mvc;

namespace Aranea.Api.Core.WebApi
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    { }
}