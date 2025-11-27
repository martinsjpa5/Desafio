
using Application.ViewModels.Requests;
using Application.ViewModels.Response;

namespace Application.Interfaces
{
    public interface ICompraService
    {
        Task<CommonResponse> CancelarAsync(int id);
        Task<CommonResponse> EfetuarAsync(IEnumerable<CompraEfetuarRequest> request);
        Task<CommonGenericResponse<IEnumerable<CompraListarResponse>>> ListarAsync();
        Task<CommonGenericResponse<IEnumerable<CompraListarResponse>>> ListarPorUsuarioAsync();
    }
}
