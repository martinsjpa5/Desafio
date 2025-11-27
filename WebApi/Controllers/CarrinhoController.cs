using Application.Interfaces;
using Application.Services;
using Application.ViewModels;
using Application.ViewModels.Requests;
using Application.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class CarrinhoController : CommonController
    {

        private readonly ICarrinhoService _carrinhoService;

        public CarrinhoController(ICarrinhoService carrinhoService)
        {
            _carrinhoService = carrinhoService;
        }


        [HttpPost]
        public async Task<ActionResult<CommonResponse>> Adicionar(IEnumerable<Carrinho> request)
        {
            var response = await _carrinhoService.Adicionar(request);
            return CustomResponse(response);
        }

        [HttpGet]
        public async Task<ActionResult<CommonGenericResponse<IEnumerable<Carrinho>>>> Obter()
        {
            var response = await _carrinhoService.Obter();
            return CustomResponse(response);
        }
    }

}
