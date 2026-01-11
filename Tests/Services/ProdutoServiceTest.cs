using Application.Interfaces;
using Application.Services;
using Application.ViewModels.Response;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Messages;
using Infraestrutura.EF.Interfaces;
using Moq;

namespace Tests.Services
{
    public class ProdutoServiceTest : CommonTest
    {
        private readonly IProdutoService _produtoService;

        public ProdutoServiceTest()
        {
            _produtoService = _autoMocker.CreateInstance<ProdutoService>();
        }

        [Fact]
        public async Task Adicionar_SemErro_RetornaSucesso()
        {
            CommonResponse result = await _produtoService.AdicionarAsync(new ProdutoAdicionarRequest());

            Assert.True(result.Sucesso);
        }

        [Fact]
        public async Task Deletar_ExisteVenda_RetornaErro()
        {
            _autoMocker.GetMock<IProdutoVendaRepository>().Setup(x => x.ExisteVendaParaProdutoAsync(It.IsAny<int>())).ReturnsAsync(true);

            CommonResponse result = await _produtoService.DeletarAsync(1);

            string msgErro = "Produto tem uma venda vinculada portanto não pode ser excluido.";

            Assert.False(result.Sucesso);
            Assert.Equal(msgErro, result.Erros.First());
        }

        [Fact]
        public async Task Deletar_ProdutoNaoEncontrado_RetornaErro()
        {
            _autoMocker.GetMock<IProdutoVendaRepository>().Setup(x => x.ExisteVendaParaProdutoAsync(It.IsAny<int>())).ReturnsAsync(false);

            CommonResponse result = await _produtoService.DeletarAsync(1);

            string msgErro = "Produto não encontrado!";

            Assert.False(result.Sucesso);
            Assert.Equal(msgErro, result.Erros.First());
        }

        [Fact]
        public async Task Deletar_SemErro_RetornaSucesso()
        {
            _autoMocker.GetMock<IProdutoVendaRepository>().Setup(x => x.ExisteVendaParaProdutoAsync(It.IsAny<int>())).ReturnsAsync(false);


            _autoMocker.GetMock<ICommonEfRepository>().Setup(x => x.ObterPorIdAsync<Produto>(It.IsAny<int>())).ReturnsAsync(new Produto());

            var result = await _produtoService.DeletarAsync(1);

            Assert.True(result.Sucesso);
        }

        [Fact]
        public async Task Editar_ProdutoNaoEncontrado_RetornaErro()
        {

            var result = await _produtoService.EditarAsync(new() { Id = 1, Nome = "1", QuantidadeEstoque = 50, Valor = 100});

            var msgErro = "Produto não encontrado!";

            Assert.False(result.Sucesso);
            Assert.Equal(msgErro, result.Erros.First());
        }

        [Fact]
        public async Task Editar_SemErro_RetornaSucesso()
        {
            _autoMocker.GetMock<ICommonEfRepository>().Setup(x => x.ObterPorIdAsync<Produto>(It.IsAny<int>())).ReturnsAsync(new Produto());

            var result = await _produtoService.EditarAsync(new() { Id = 1, Nome = "1", QuantidadeEstoque = 50, Valor = 100 });

            Assert.True(result.Sucesso);
        }

        [Fact]
        public async Task Listar_SemErro_RetornaSucesso()
        {
            _autoMocker.GetMock<ICommonEfRepository>().Setup(x => x.ObterTodosAsync<Produto>()).ReturnsAsync([]);

            var result = await _produtoService.ListarAsync();

            Assert.True(result.Sucesso);
        }
    }
}
