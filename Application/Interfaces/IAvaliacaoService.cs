
using Application.ViewModels.Requests;
using Application.ViewModels.Response;

namespace Application.Interfaces
{
    public interface IAvaliacaoService
    {
        Task<CommonResponse> AvaliarProdutoAsync(AvaliarProdutoRequest request);
        Task<CommonGenericResponse<IEnumerable<AvaliacaoListarResponse>>> ListarAsync();
    }
}
