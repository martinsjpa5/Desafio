
using Application.ViewModels;
using Application.ViewModels.Requests;
using Application.ViewModels.Response;

namespace Application.Interfaces
{
    public interface IProdutoService
    {
        Task<CommonResponse> AdicionarAsync(ProdutoAdicionarRequest request);
        Task<CommonResponse> EditarAsync(ProdutoEditarRequest request);
        Task<CommonResponse> DeletarAsync(int id);
        Task<CommonGenericResponse<IEnumerable<ProdutoListar>>> ListarAsync();
    }
}
