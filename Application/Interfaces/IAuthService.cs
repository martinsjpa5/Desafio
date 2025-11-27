using Application.ViewModels.Requests;
using Application.ViewModels.Response;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<CommonResponse> Registrar(RegistroRequest request);
        Task<CommonGenericResponse<string>> Login(LoginRequest request);
    }
}
