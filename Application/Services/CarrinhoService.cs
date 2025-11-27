
using System.Security.Claims;
using Application.Interfaces;
using Application.ViewModels;
using Application.ViewModels.Response;
using Domain.Interfaces.Caching;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public class CarrinhoService : ICarrinhoService
    {
        private readonly ICommonCachingRepository _commonCachingRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CarrinhoService(ICommonCachingRepository commonCachingRepository, IHttpContextAccessor httpContextAccessor)
        {
            _commonCachingRepository = commonCachingRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CommonResponse> Adicionar(IEnumerable<Carrinho> request)
        {
            string usuarioId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("userId")!;

            CarrinhoModel carrinhoModel = new()
            {
                 UsuarioId = usuarioId,
                 Itens = request.Select(x => new ItensCarrinhoModel() { 
                     Produto = new ProdutoCarrinhoModel { Id = x.Produto.Id, Nome = x.Produto.Nome, QuantidadeEstoque = x.Produto.QuantidadeEstoque, Valor = x.Produto.Valor },
                     Quantidade = x.Quantidade
                 }).ToList()
            };

            await _commonCachingRepository.SetAsync(carrinhoModel, TimeSpan.FromHours(1));

            return CommonResponse.SucessoBuilder();
        }

        public async Task<CommonGenericResponse<IEnumerable<Carrinho>>> Obter()
        {
            string usuarioId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("userId")!;
            CarrinhoModel carrinho = new() { UsuarioId = usuarioId };

            CarrinhoModel? carrinhoModel = await _commonCachingRepository.GetAsync<CarrinhoModel>(carrinho.ObterKey());

            if (carrinhoModel == null)
                return CommonGenericResponse<IEnumerable<Carrinho>>.SucessoBuilder([]);

            List<Carrinho> carrinhoItens = carrinhoModel.Itens.Select(x => new Carrinho { Quantidade = x.Quantidade, Produto = new ProdutoListar { Id = x.Produto.Id, Nome = x.Produto.Nome, QuantidadeEstoque = x.Produto.QuantidadeEstoque, Valor = x.Produto.Valor} }).ToList();

            return CommonGenericResponse<IEnumerable<Carrinho>>.SucessoBuilder(carrinhoItens);

        }
    }
}
