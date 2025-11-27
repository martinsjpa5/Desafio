
namespace Domain.Entities
{
    public class Avaliacao : Entity
    {
        public int Nota { get; set; }
        public string Comentario { get; set; }
        public Produto Produto { get; set; }
        public int ProdutoId { get; set; }
        public string UsuarioId { get; set; }
    }
}
