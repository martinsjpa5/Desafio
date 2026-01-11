
using Application.Interfaces;
using Application.Services;
using Application.ViewModels;
using Application.ViewModels.Requests;
using Application.ViewModels.Response;
using Domain.Entities;
using Infraestrutura.EF.Interfaces;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Linq.Expressions;

namespace Tests.Services
{
    public class CompraServiceTest : CommonTest
    {
        private readonly ICompraService _compraService;

        public CompraServiceTest()
        {
            _compraService = _autoMocker.CreateInstance<CompraService>();
        }

        [Fact]
        public async Task Cancelar_VendaNaoEncontrada_DeveRetornarFalha()
        {
            CommonResponse result = await _compraService.CancelarAsync(1);

            Assert.False(result.Sucesso);
        }

        [Fact]
        public async Task Cancelar_VendaEncontrada_DeveRetornarSucesso()
        {
            _autoMocker.GetMock<ICommonEfRepository>()
                .Setup(r => r.ObterPorIdAsync<Compra>(
                    It.IsAny<int>(),
                    It.IsAny<Func<IQueryable<Compra>, IIncludableQueryable<Compra, object>>[]>()
            ))
            .ReturnsAsync(new Compra { UsuarioId = "1", Itens = [new() { Quantidade = 2, Produto = new() { QuantidadeEstoque = 5 } }] });

            CommonResponse result = await _compraService.CancelarAsync(1);
            Assert.True(result.Sucesso);
        }

        [Fact]
        public async Task Efetuar_SemErro_DeveRetornarSucesso()
        {
            IEnumerable<CompraEfetuarRequest> request = [new() { ProdutoId = 1, Quantidade = 2 }];

            ICollection<Produto> produtos = [new() { Valor = 1, QuantidadeEstoque = 3, Id = 1 }];

            _autoMocker.GetMock<ICommonEfRepository>().Setup(x => x.ObterTodosPorCondicaoAsync<Produto>(It.IsAny<Expression<Func<Produto, bool>>>())).ReturnsAsync(produtos);

            var result = await _compraService.EfetuarAsync(request);

            Assert.True(result.Sucesso);

        }

        [Fact]
        public async Task Efetuar_ProdutoNaoEncontrado_DeveRetornarFalha()
        {
            IEnumerable<CompraEfetuarRequest> request = [new() { ProdutoId = 1, Quantidade = 2 }];

            ICollection<Produto> produtos = [new() { Valor = 1, QuantidadeEstoque = 3, Id = 1 }];

            _autoMocker.GetMock<ICommonEfRepository>().Setup(x => x.ObterTodosPorCondicaoAsync<Produto>(It.IsAny<Expression<Func<Produto, bool>>>())).ReturnsAsync([]);

            var result = await _compraService.EfetuarAsync(request);

            string erroProdutoNaoEncontrado = "Produto não encontrado";

            Assert.False(result.Sucesso);
            Assert.Equal(erroProdutoNaoEncontrado, result.Erros.First());
        }

        [Fact]
        public async Task Efetuar_ProdutoSemEstoque_DeveRetornarFalha()
        {
            IEnumerable<CompraEfetuarRequest> request = [new() { ProdutoId = 1, Quantidade = 2 }];

            ICollection<Produto> produtos = [new() { Valor = 1, QuantidadeEstoque = 1, Id = 1 }];

            _autoMocker.GetMock<ICommonEfRepository>().Setup(x => x.ObterTodosPorCondicaoAsync<Produto>(It.IsAny<Expression<Func<Produto, bool>>>())).ReturnsAsync(produtos);

            var result = await _compraService.EfetuarAsync(request);

            string erroProdutoSemEstoque = "Estoque insuficiente para o produto";

            Assert.False(result.Sucesso);
            Assert.Contains(erroProdutoSemEstoque, result.Erros.First());
        }

        [Fact]
        public async Task Listar_SemErro_DeveRetornarSucesso()
        {

            _autoMocker.GetMock<ICommonEfRepository>().Setup(x => x.ObterTodosAsync<Compra>(It.IsAny<Func<IQueryable<Compra>, IIncludableQueryable<Compra, object>>[]>())).ReturnsAsync([]);

            var result = await _compraService.ListarAsync();

            Assert.True(result.Sucesso);
        }

        [Fact]
        public async Task ListarPorUsuario_SemErro_DeveRetornarSucesso()
        {

            _autoMocker.GetMock<ICommonEfRepository>().Setup(x => x.ObterTodosPorCondicaoAsync<Compra>(It.IsAny<Expression<Func<Compra, bool>>>(),
        It.IsAny<Func<IQueryable<Compra>, IIncludableQueryable<Compra, object>>[]>())).ReturnsAsync([]);

            var result = await _compraService.ListarPorUsuarioAsync();

            Assert.True(result.Sucesso);
        }
    }
}
