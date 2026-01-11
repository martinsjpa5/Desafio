using Application.Interfaces;
using Application.ViewModels.Response;
using Domain.Interfaces.Repositories;
using Domain.Messages;
using Domain.Models;

namespace Application.Services
{
    public class RelatorioService : IRelatorioService
    {
        private readonly IRelatorioDapperRepository _relatorioDapperRepository;
        private readonly ICommonCachingRepository _commonCachingRepository;

        public RelatorioService(IRelatorioDapperRepository relatorioDapperRepository, ICommonCachingRepository commonCachingRepository)
        {
            _relatorioDapperRepository = relatorioDapperRepository;
            _commonCachingRepository = commonCachingRepository;
        }

        public async Task<CommonResponse> Gerar(FiltroRelatorio filtro)
        {
            RelatorioModel relatorio = await _relatorioDapperRepository.ObterRelatorioPorProdutoAsync(filtro.DataInicial, filtro.DataFinal);

            await _commonCachingRepository.SetAsync(relatorio, TimeSpan.FromHours(1));

            return CommonResponse.SucessoBuilder();
        }

        public async Task<CommonGenericResponse<IEnumerable<RelatorioResponse>>> Obter()
        {

            RelatorioModel? relatorio = await _commonCachingRepository.GetAsync<RelatorioModel>(new RelatorioModel().ObterKey());

            if (relatorio == null)
                return CommonGenericResponse<IEnumerable<RelatorioResponse>>.SucessoBuilder([]);

            List<RelatorioResponse> produtos = relatorio.Produtos.Select(relatorio => new RelatorioResponse { Estoque = relatorio.Estoque, NomeProduto = relatorio.NomeProduto, ValorTotal = relatorio.ValorTotal, Vendidos = relatorio.Vendidos, Canceladas = relatorio.Canceladas }).ToList();

            return CommonGenericResponse<IEnumerable<RelatorioResponse>>.SucessoBuilder(produtos);

        }
    }
}
