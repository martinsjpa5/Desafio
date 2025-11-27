
using Application.ViewModels;
using Application.ViewModels.Response;

namespace Application.Interfaces
{
    public interface ICarrinhoService
    {
        public Task<CommonResponse> Adicionar(IEnumerable<Carrinho> request);
        Task<CommonGenericResponse<IEnumerable<Carrinho>>> Obter();


    }
}
