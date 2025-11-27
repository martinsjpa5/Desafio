
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Requests
{
    public class ProdutoEditarRequest
    {
        [Required(ErrorMessage = "O Id do produto é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O Id deve ser maior que zero.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do produto é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome do produto deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "A quantidade em estoque deve ser zero ou maior.")]
        public int QuantidadeEstoque { get; set; }

        [Range(0.01, 999999.99, ErrorMessage = "O valor deve ser maior que zero.")]
        public decimal Valor { get; set; }
    }
}
