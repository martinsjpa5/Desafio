using Application.Interfaces;
using Application.Services;
using Application.ViewModels;
using Application.ViewModels.Response;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Moq;

namespace Tests.Services
{
    public class CarrinhoServiceTest : CommonTest
    {
        private readonly ICarrinhoService _carrinhoService;

        public CarrinhoServiceTest()
        {
            _carrinhoService = _autoMocker.CreateInstance<CarrinhoService>();
        }
        [Fact]
        public async Task Adicionar_semErro_DeveRetornarSucesso()
        {
            IEnumerable<Carrinho> request = [new() { Produto = new() { Id = 1, Nome = "Teste", QuantidadeEstoque = 1, Valor = 2}, Quantidade =1}];

            CommonResponse result = await _carrinhoService.Adicionar(request);

            Assert.True(result.Sucesso);
        }

        [Fact]
        public async Task Obter_comItens_DeveRetornarSucesso()
        {
            CarrinhoModel carrinho = new() { UsuarioId = "1", Itens = [new() { Produto = new() { Id = 1, Nome = "Teste", QuantidadeEstoque = 1, Valor = 2 }, Quantidade = 2 }] };

            _autoMocker.GetMock<ICommonCachingRepository>().Setup(x => x.GetAsync<CarrinhoModel>(It.IsAny<string>())).ReturnsAsync(carrinho);

            CommonGenericResponse<IEnumerable<Carrinho>> result = await _carrinhoService.Obter();

            Assert.True(result.Sucesso);
        }

        [Fact]
        public async Task Obter_semItens_DeveRetornarSucesso()
        {
            CommonGenericResponse<IEnumerable<Carrinho>> result = await _carrinhoService.Obter();

            Assert.True(result.Sucesso);
            Assert.Empty(result.Data);
        }
    }
}