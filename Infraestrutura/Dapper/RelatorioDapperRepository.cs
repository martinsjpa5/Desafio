
using Dapper;
using System.Data;
using Domain.Interfaces.Repositories;
using Domain.Models;

namespace Infraestrutura.Dapper
{
    public class RelatorioDapperRepository : IRelatorioDapperRepository
    {
        private readonly ICommonRepositoryDapper _commonRepository;

        public RelatorioDapperRepository(ICommonRepositoryDapper commonRepositoryDapper)
        {
            _commonRepository = commonRepositoryDapper;
        }

        public async Task<RelatorioModel> ObterRelatorioPorProdutoAsync(DateTime dataInicial, DateTime dataFinal)
        {
            string query = @"
            SELECT 
                p.Nome AS NomeProduto,
                p.QuantidadeEstoque AS Estoque,
            
                COALESCE(SUM(CASE WHEN c.Cancelada = 0 THEN iv.Quantidade ELSE 0 END), 0) AS Vendidos,
                COALESCE(SUM(CASE WHEN c.Cancelada = 1 THEN iv.Quantidade ELSE 0 END), 0) AS Canceladas,
                COALESCE(SUM(CASE WHEN c.Cancelada = 0 THEN iv.Quantidade * iv.Valor ELSE 0 END), 0) AS ValorTotal
            
            FROM Produtos p
            LEFT JOIN Itens iv 
                ON iv.ProdutoId = p.Id
            LEFT JOIN Compras c 
                ON c.Id = iv.CompraId
               AND c.DataCriacao >= @DataInicial
               AND c.DataCriacao < DATEADD(DAY, 1, @DataFinal)
            
            GROUP BY 
                p.Nome, p.QuantidadeEstoque
            ORDER BY 
                p.Nome;";
            
            var parameters = new DynamicParameters();
            parameters.Add("DataInicial", dataInicial, DbType.DateTime);
            parameters.Add("DataFinal", dataFinal, DbType.DateTime);

            var produtos = await _commonRepository.QueryAsync<ProdutoRelatorioModel>(query, parameters);

            return new RelatorioModel { Produtos = produtos };
        }




    }
}
