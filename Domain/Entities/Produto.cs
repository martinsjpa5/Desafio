
namespace Domain.Entities
{
    public class Produto: Entity
    {
        public string Nome { get; set; }
        public decimal Valor { get; set; }
        public int QuantidadeEstoque { get; set; }
    }
}
