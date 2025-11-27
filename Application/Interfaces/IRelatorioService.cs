
using Application.ViewModels.Response;
using Domain.Messages;

namespace Application.Interfaces
{
    public interface IRelatorioService
    {
        Task<CommonResponse> Gerar(FiltroRelatorio filtro);
        Task<CommonGenericResponse<IEnumerable<RelatorioResponse>>> Obter();
    }
}
