
namespace Application.ViewModels.Response
{
    public class CompraListarResponse
    {
        public int Id { get; set; }
        public bool Cancelada { get; set; }
        public decimal ValorTotal { get; set; }
        public IEnumerable<ItemListarResponse> Itens { get; set; }
    }
}
