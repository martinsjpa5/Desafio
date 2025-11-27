using Application.Interfaces;
using Application.ViewModels;
using Application.ViewModels.Requests;
using Application.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{

    [Route("[controller]")]
    [Authorize]
    public class ProdutoController : CommonController
    {
        private readonly IProdutoService _produtoService;

        public ProdutoController(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        [HttpGet]
        public async Task<ActionResult<CommonGenericResponse<IEnumerable<ProdutoListar>>>> Listar()
        {
            var response = await _produtoService.ListarAsync();
            return CustomResponse(response);
        }

        [Authorize(Roles = "Gerente")]
        [HttpPost]
        public async Task<ActionResult<CommonResponse>> Adicionar(ProdutoAdicionarRequest request)
        {
            var response = await _produtoService.AdicionarAsync(request);
            return CustomResponse(response);
        }

        [Authorize(Roles = "Gerente")]
        [HttpPut]
        public async Task<ActionResult<CommonResponse>> Editar(ProdutoEditarRequest request)
        {
            var response = await _produtoService.EditarAsync(request);
            return CustomResponse(response);
        }

        [Authorize(Roles = "Gerente")]
        [HttpDelete("{produtoId}")]
        public async Task<ActionResult<CommonResponse>> Delete(int produtoId)
        {
            var response = await _produtoService.DeletarAsync(produtoId);
            return CustomResponse(response);
        }
    }
}
