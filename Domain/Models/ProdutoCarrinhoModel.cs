
namespace Domain.Models
{
    public class ProdutoCarrinhoModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int QuantidadeEstoque { get; set; }
        public decimal Valor { get; set; }
    }
}
