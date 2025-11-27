using Application.ViewModels.Response;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    public class CommonController : ControllerBase
    {
        protected ActionResult CustomResponse(CommonResponse response)
        {
            if (response.Erros.Count > 0)
                return BadRequest(response);
            else
                return Ok(response);
        }
        protected ActionResult CustomResponse<T>(CommonGenericResponse<T> response)
        {
            if (response.Erros.Count > 0)
                return BadRequest(response);
            else
                return Ok(response);
        }

    }
}
