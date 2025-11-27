using Application.Interfaces;
using Application.ViewModels.Requests;
using Application.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class CompraController : CommonController
    {

        private readonly ICompraService _compraService;

        public CompraController(ICompraService compraService)
        {
            _compraService = compraService;
        }

        [Authorize(Roles = "Gerente")]
        [HttpGet("admin")]
        public async Task<ActionResult<CommonGenericResponse<IEnumerable<CompraListarResponse>>>> Listar()
        {
            var response = await _compraService.ListarAsync();
            return CustomResponse(response);
        }

        [HttpGet("usuario")]
        public async Task<ActionResult<CommonGenericResponse<IEnumerable<CompraListarResponse>>>> ListarPorUsuario()
        {
            var response = await _compraService.ListarPorUsuarioAsync();
            return CustomResponse(response);
        }

        [HttpPost]
        public async Task<ActionResult<CommonResponse>> Efetuar(IEnumerable<CompraEfetuarRequest> request)
        {
            var response = await _compraService.EfetuarAsync(request);
            return CustomResponse(response);
        }

        [Authorize(Roles = "Gerente")]
        [HttpDelete("{compraId}")]
        public async Task<ActionResult<CommonResponse>> Cancelar(int compraId)
        {
            var response = await _compraService.CancelarAsync(compraId);
            return CustomResponse(response);
        }
    }
}
