
using Domain.Interfaces.Repositories;
using Infraestrutura.EF.Context;
using Microsoft.EntityFrameworkCore;

namespace Infraestrutura.EF.Repositories
{
    public class ProdutoVendaRepository : IProdutoVendaRepository
    {
        private readonly AppDbContext _context;

        public ProdutoVendaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExisteVendaParaProdutoAsync(int produtoId)
        {
            return await _context.Itens
                .AnyAsync(pv => pv.Produto.Id == produtoId);
        }
    }
}
