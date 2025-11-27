
namespace Domain.Entities
{
    public class Item : Entity
    {
        public int Quantidade { get; set; }
        public decimal Valor { get; set; }
        public Produto Produto { get; set; }
        public Compra Compra { get; set; }
    }
}
