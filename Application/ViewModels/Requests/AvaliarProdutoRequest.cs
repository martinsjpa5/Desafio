
namespace Application.ViewModels.Requests
{
    public class AvaliarProdutoRequest
    {
        public int ProdutoId { get; set; }
        public int Nota { get; set; }
        public string Comentario { get; set; }
    }
}
