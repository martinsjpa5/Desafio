using Application.Interfaces;
using Application.ViewModels.Requests;
using Application.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class AvaliacaoController : CommonController
    {
        private readonly IAvaliacaoService _avaliacaoService;

        public AvaliacaoController(IAvaliacaoService avaliacaoService)
        {
            _avaliacaoService = avaliacaoService;
        }


        [HttpPost]
        public async Task<ActionResult<CommonResponse>> Avaliar(AvaliarProdutoRequest request)
        {
            return await _avaliacaoService.AvaliarProdutoAsync(request);
        }

        [HttpGet]
        public async Task<ActionResult<CommonGenericResponse<IEnumerable<AvaliacaoListarResponse>>>> Listar()
        {
            return await _avaliacaoService.ListarAsync();
        }
    }
}
