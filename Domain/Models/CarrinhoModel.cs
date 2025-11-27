
using Domain.Interfaces.Caching;

namespace Domain.Models
{
    public class CarrinhoModel : ICommonCaching
    {
        public required string UsuarioId { get; set; }
        public IEnumerable<ItensCarrinhoModel> Itens { get; set; }
        public string ObterKey()
        {
            return nameof(CarrinhoModel) + ":" + UsuarioId;
        }
    }
}
