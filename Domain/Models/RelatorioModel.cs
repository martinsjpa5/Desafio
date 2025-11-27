
using Domain.Interfaces.Caching;

namespace Domain.Models
{
    public class RelatorioModel : ICommonCaching
    {

        public IEnumerable<ProdutoRelatorioModel> Produtos { get; set; }

        public string ObterKey()
        {
            return nameof(RelatorioModel);
        }
    }
}
