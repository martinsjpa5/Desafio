
using Application.Interfaces;
using Application.ViewModels;
using Application.ViewModels.Requests;
using Application.ViewModels.Response;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infraestrutura.EF.Interfaces;

namespace Application.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly ICommonEfRepository _commonRepository;
        private readonly IProdutoVendaRepository _produtoVendaRepository;

        public ProdutoService(ICommonEfRepository commonRepository, IProdutoVendaRepository produtoVendaRepository)
        {
            _commonRepository = commonRepository;
            _produtoVendaRepository = produtoVendaRepository;
        }

        public async Task<CommonResponse> AdicionarAsync(ProdutoAdicionarRequest request)
        {
            Produto produto = new Produto { Nome = request.Nome, QuantidadeEstoque = request.QuantidadeEstoque, Valor = request.Valor };
            await _commonRepository.AdicionarEntityAsync(produto);
            await _commonRepository.SalvarAlteracoesAsync();

            return CommonResponse.SucessoBuilder();
        }

        public async Task<CommonResponse> DeletarAsync(int id)
        {
            bool existeVenda = await _produtoVendaRepository.ExisteVendaParaProdutoAsync(id);
            if (existeVenda)
            {
                return CommonResponse.ErroBuilder("Produto tem uma venda vinculada portanto não pode ser excluido.");
            }

            Produto? entidade = await _commonRepository.ObterPorIdAsync<Produto>(id);

            if (entidade == null)
            {
                return CommonResponse.ErroBuilder("Produto não encontrado!");
            }

            _commonRepository.DeletarEntity(entidade);

            await _commonRepository.SalvarAlteracoesAsync();

            return CommonResponse.SucessoBuilder();
        }

        public async Task<CommonResponse> EditarAsync(ProdutoEditarRequest request)
        {
            Produto? entidade = await _commonRepository.ObterPorIdAsync<Produto>(request.Id);

            if (entidade == null)
                return CommonResponse.ErroBuilder("Produto não encontrado!");

            entidade.Valor =  request.Valor;
            entidade.Nome = request.Nome;
            entidade.QuantidadeEstoque = request.QuantidadeEstoque;

            await _commonRepository.SalvarAlteracoesAsync();

            return CommonResponse.SucessoBuilder();
        }

        public async Task<CommonGenericResponse<IEnumerable<ProdutoListar>>> ListarAsync()
        {
            ICollection<Produto> produtos = await _commonRepository.ObterTodosAsync<Produto>();

            IEnumerable<ProdutoListar> produtosResponse = produtos.Select(produto => new ProdutoListar { Id = produto.Id, Nome = produto.Nome, Valor = produto.Valor, QuantidadeEstoque = produto.QuantidadeEstoque }).ToList();

            return CommonGenericResponse<IEnumerable<ProdutoListar>>.SucessoBuilder(produtosResponse);
            
        }
    }
}
