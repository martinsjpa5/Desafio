
namespace Domain.Entities
{
    public class Compra : Entity
    {
        public ICollection<Item> Itens { get; set; } = new List<Item>();
        public decimal ValorTotal { get; set; }
        public required string UsuarioId { get; set; }
        public bool Cancelada { get; set; }

        public Compra()
        {
            Cancelada = false;
        }

        public void CancelarVenda()
        {
            foreach (var item in Itens)
            {
                item.Produto.QuantidadeEstoque += item.Quantidade;
            }
            Cancelada = true;
        }
    }
}
