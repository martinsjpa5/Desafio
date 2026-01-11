
namespace Application.ViewModels.Response
{
    public class RelatorioResponse
    {
        public string NomeProduto { get; set; }
        public int Estoque { get; set; }
        
        public int Vendidos { get; set; }
        public int Canceladas { get; set; }
        public decimal ValorTotal { get; set; }
    }
}
