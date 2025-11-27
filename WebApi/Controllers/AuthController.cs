using Application.Interfaces;
using Application.ViewModels.Requests;
using Application.ViewModels.Response;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    public class AuthController : CommonController
    {

        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("registrar")]
        public async Task<ActionResult<CommonResponse>> Registrar(RegistroRequest request)
        {
            var response = await _authService.Registrar(request);
            return CustomResponse(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<CommonGenericResponse<string>>> Login(LoginRequest request)
        {
            var response = await _authService.Login(request);
            return CustomResponse(response);
        }
    }
}
