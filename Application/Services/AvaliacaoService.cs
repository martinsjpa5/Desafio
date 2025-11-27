
using System.Linq;
using System.Security.Claims;
using Application.Interfaces;
using Application.ViewModels.Requests;
using Application.ViewModels.Response;
using Domain.Entities;
using Infraestrutura.EF.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class AvaliacaoService : IAvaliacaoService
    {
        private readonly ICommonEfRepository _commonRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AvaliacaoService(ICommonEfRepository commonRepository, IHttpContextAccessor httpContextAccessor)
        {
            _commonRepository = commonRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CommonResponse> AvaliarProdutoAsync(AvaliarProdutoRequest request)
        {
            string usuarioId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            Compra? venda = await _commonRepository.ObterPorCondicaoAsync<Compra>(venda => venda.UsuarioId == usuarioId && venda.Itens.Any(pv => pv.Produto.Id == request.ProdutoId));

            if (venda == null)
            {
                return CommonResponse.ErroBuilder("não é possível avaliar um produto que você não comprou");
            }

            Avaliacao? avaliacaoProdutoUsuario = await _commonRepository.ObterPorCondicaoAsync<Avaliacao>(avaliacao => avaliacao.UsuarioId == usuarioId);

            if (avaliacaoProdutoUsuario != null)
            {
                return CommonResponse.ErroBuilder("Só é possível avaliar o produto uma vez.");
            }

            Avaliacao avaliacao = new() { Comentario = request.Comentario, Nota = request.Nota, ProdutoId = request.ProdutoId, UsuarioId = usuarioId };

            await _commonRepository.AdicionarEntityAsync(avaliacao);
            await _commonRepository.SalvarAlteracoesAsync();

            return CommonResponse.SucessoBuilder();
        }

        public async Task<CommonGenericResponse<IEnumerable<AvaliacaoListarResponse>>> ListarAsync()
        {
            ICollection<Avaliacao> avaliacoes = await _commonRepository.ObterTodosAsync<Avaliacao>(query => query.Include(x => x.Produto));

            List<AvaliacaoListarResponse> response = avaliacoes.Select(avaliacao => new AvaliacaoListarResponse
            {
                Comentario = avaliacao.Comentario,
                Nota = avaliacao.Nota,
                ProdutoId = avaliacao.Produto.Id
            }).ToList();

            return CommonGenericResponse<IEnumerable<AvaliacaoListarResponse>>.SucessoBuilder(response);
        }
    }
}