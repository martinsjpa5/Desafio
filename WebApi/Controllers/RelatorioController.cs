using Application.Interfaces;
using Application.Services;
using Application.ViewModels.Response;
using Application.ViewModels;
using Domain.Interfaces.Queues;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.Messages;
using Application.ViewModels.Requests;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class RelatorioController : CommonController
    {
        private readonly IRelatorioService _relatorioService;
        private readonly IRelatorioQueue _relatorioQueue;

        public RelatorioController(IRelatorioService relatorioService, IRelatorioQueue relatorioQueue)
        {
            _relatorioService = relatorioService;
            _relatorioQueue = relatorioQueue;
        }

        [Authorize(Roles = "Gerente")]
        [HttpPost]
        public async Task<ActionResult<CommonGenericResponse<IEnumerable<ProdutoListar>>>> Solicitar(SolicitarRelatorioRequest request)
        {
            await _relatorioQueue.PublishMessageAsync(new FiltroRelatorio { DataInicial = request.DataInicial, DataFinal = request.DataFinal});
            return CustomResponse(CommonResponse.SucessoBuilder());
        }
        [Authorize(Roles = "Gerente")]
        [HttpGet]
        public async Task<ActionResult<CommonGenericResponse<IEnumerable<RelatorioResponse>>>> Listar()
        {
            var response = await _relatorioService.Obter();
            return CustomResponse(response);
        }
    }
}
