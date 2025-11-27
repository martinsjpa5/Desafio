
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
            ISNULL(SUM(iv.Quantidade), 0) AS Vendidos,
            ISNULL(SUM(iv.Quantidade * iv.Valor), 0) AS ValorTotal
        FROM Produtos p
        LEFT JOIN Itens iv ON iv.ProdutoId = p.Id
        LEFT JOIN Compras c ON c.Id = iv.CompraId 
                           AND c.Cancelada = 0
                           AND c.DataCriacao BETWEEN @DataInicial AND @DataFinal
        GROUP BY 
            p.Nome, p.QuantidadeEstoque
        ORDER BY 
            p.Nome";

            DynamicParameters parameters = new();
            parameters.Add("DataInicial", dataInicial, DbType.DateTime);
            parameters.Add("DataFinal", dataFinal, DbType.DateTime);

            IEnumerable<ProdutoRelatorioModel> produtos = await _commonRepository.QueryAsync<ProdutoRelatorioModel>(query, parameters);

            return new RelatorioModel
            {
                Produtos = produtos
            };
        }

    }
}
