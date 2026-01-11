
using System.Security.Claims;
using Application.Interfaces;
using Application.ViewModels;
using Application.ViewModels.Requests;
using Application.ViewModels.Response;
using Domain.Entities;
using Infraestrutura.EF.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class CompraService : ICompraService
    {
        private readonly ICommonEfRepository _commonRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICarrinhoService _carrinhoService;

        public CompraService(ICommonEfRepository commonRepository, ICarrinhoService carrinhoService, IHttpContextAccessor httpContextAccessor)
        {
            _commonRepository = commonRepository;
            _carrinhoService = carrinhoService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CommonResponse> CancelarAsync(int id)
        {
            Compra? entidade = await _commonRepository.ObterPorIdAsync<Compra>(id, query => query.Include(x => x.Itens).ThenInclude(x => x.Produto));

            if (entidade == null)
            {
                return CommonResponse.ErroBuilder("Venda não encontrada!");
            }

            entidade.CancelarVenda();

            await _commonRepository.SalvarAlteracoesAsync();

            return CommonResponse.SucessoBuilder();
        }

        public async Task<CommonResponse> EfetuarAsync(IEnumerable<CompraEfetuarRequest> request)
        {
            List<int> produtoIds = request.Select(x => x.ProdutoId).ToList();

            ICollection<Produto> produtos = await _commonRepository
                .ObterTodosPorCondicaoAsync<Produto>(x => produtoIds.Contains(x.Id));
            string usuarioId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("userId")!;
           
            Compra compra = new() { UsuarioId = usuarioId };


            foreach (var item in request)
            {
                Produto? produto = produtos.FirstOrDefault(x => x.Id == item.ProdutoId);

                if (produto == null)
                    return CommonResponse.ErroBuilder("Produto não encontrado");

                if (produto.QuantidadeEstoque < item.Quantidade)
                    return CommonResponse.ErroBuilder($"Estoque insuficiente para o produto {produto.Nome}");

                produto.QuantidadeEstoque -= item.Quantidade;

                compra.Itens.Add(new Item
                {
                    Produto = produto,
                    Quantidade = item.Quantidade,
                    Valor = produto.Valor
                });

                compra.ValorTotal += item.Quantidade * produto.Valor;
            }

            await _commonRepository.AdicionarEntityAsync(compra);
            await _commonRepository.SalvarAlteracoesAsync();

            List<Carrinho> auxLimparCarrinho = [];
            await _carrinhoService.Adicionar(auxLimparCarrinho);

            return CommonResponse.SucessoBuilder();
        }

        public async Task<CommonGenericResponse<IEnumerable<CompraListarResponse>>> ListarAsync()
        {
            ICollection<Compra> compras = await _commonRepository.ObterTodosAsync<Compra>(query => query.Include(x => x.Itens).ThenInclude(y => y.Produto));

            List<CompraListarResponse> vendasResponse = compras.Select(compra => new CompraListarResponse
            {
                Id = compra.Id,
                ValorTotal = compra.ValorTotal,
                Cancelada = compra.Cancelada,
                Itens = compra.Itens.Select(produtoVenda => new ItemListarResponse
                {
                    Nome = produtoVenda.Produto.Nome,
                    Quantidade = produtoVenda.Quantidade,
                    Valor = produtoVenda.Valor
                }).ToList(),
            }).ToList();

            return CommonGenericResponse<IEnumerable<CompraListarResponse>>.SucessoBuilder(vendasResponse);
            
        }

        public async Task<CommonGenericResponse<IEnumerable<CompraListarResponse>>> ListarPorUsuarioAsync()
        {
            string usuarioId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("userId")!;

            ICollection<Compra> compras = await _commonRepository.ObterTodosPorCondicaoAsync<Compra>(compra => compra.UsuarioId == usuarioId,
                query => query.Include(x => x.Itens).ThenInclude(y => y.Produto));

            List<CompraListarResponse> vendasResponse = compras.Select(compra => new CompraListarResponse
            {
                Id = compra.Id,
                ValorTotal = compra.ValorTotal,
                Cancelada = compra.Cancelada,
                Itens = compra.Itens.Select(produtoVenda => new ItemListarResponse
                {
                    Nome = produtoVenda.Produto.Nome,
                    Quantidade = produtoVenda.Quantidade,
                    Valor = produtoVenda.Valor
                }).ToList(),
            }).ToList();

            return CommonGenericResponse<IEnumerable<CompraListarResponse>>.SucessoBuilder(vendasResponse);

        }
    }
}
