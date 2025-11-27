
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Requests
{
    public class SolicitarRelatorioRequest : IValidatableObject
    {
        [Required(ErrorMessage = "A Data Inicial é obrigatória.")]
        [DataType(DataType.Date)]
        public DateTime DataInicial { get; set; }

        [Required(ErrorMessage = "A Data Final é obrigatória.")]
        [DataType(DataType.Date)]
        public DateTime DataFinal { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DataFinal < DataInicial)
            {
                yield return new ValidationResult(
                    "A Data Final não pode ser menor que a Data Inicial.",
                    new[] { nameof(DataFinal), nameof(DataInicial) }
                );
            }
        }
    }
}
