
namespace Domain.Models
{
    public class ProdutoRelatorioModel
    {
        public string NomeProduto { get; set; }
        public int Estoque { get; set; }

        public int Vendidos { get; set; }
        public decimal ValorTotal { get; set; }
    }
}
