
namespace Domain.Interfaces.Repositories
{
    public interface IProdutoVendaRepository
    {
        Task<bool> ExisteVendaParaProdutoAsync(int ind);
    }
}
