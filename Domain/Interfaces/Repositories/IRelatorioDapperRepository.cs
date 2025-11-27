
using Domain.Models;

namespace Domain.Interfaces.Repositories
{
    public interface IRelatorioDapperRepository
    {
        Task<RelatorioModel> ObterRelatorioPorProdutoAsync(DateTime dataInicial, DateTime dataFinal);
    }
}
