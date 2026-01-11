
using Application.Interfaces;
using Application.Services;
using Domain.Interfaces.Repositories;
using Domain.Messages;
using Domain.Models;
using Moq;

namespace Tests.Services
{
    public class RelatorioServiceTest : CommonTest
    {
        private readonly IRelatorioService _relatorioService;
        public RelatorioServiceTest()
        {
            _relatorioService = _autoMocker.CreateInstance<RelatorioService>();
        }
        [Fact]
        public async Task Gerar_SemErro_RetornaSucesso()
        {
            var result = await _relatorioService.Gerar(new FiltroRelatorio());

            Assert.True(result.Sucesso);
        }

        [Fact]
        public async Task Obter_Vazio_RetornaSucesso()
        {
            var result = await _relatorioService.Obter();

            Assert.True(result.Sucesso);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task Obter_Preenchido_RetornaSucesso()
        {
            _autoMocker.GetMock<ICommonCachingRepository>().Setup(x => x.GetAsync<RelatorioModel>(It.IsAny<string>())).ReturnsAsync(new RelatorioModel { Produtos = [new()] });

            var result = await _relatorioService.Obter();

            Assert.True(result.Sucesso);
            Assert.NotEmpty(result.Data);
        }
    }
}