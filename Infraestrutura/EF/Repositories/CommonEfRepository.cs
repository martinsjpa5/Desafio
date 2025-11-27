using System.Linq.Expressions;
using Domain.Entities;
using Infraestrutura.EF.Context;
using Infraestrutura.EF.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Infraestrutura.EF.Repositories
{
    public class CommonEfRepository : ICommonEfRepository, IDisposable
    {
        protected readonly AppDbContext _dataContext;
        private bool disposed = false;

        public CommonEfRepository(AppDbContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<bool> AdicionarEntityAsync<T>(T Entity) where T : Entity
        {
            await _dataContext.AddAsync(Entity);
            return true;
        }
        public async Task<bool> AdicionarEntitysAsync<T>(List<T> Entitys) where T : Entity
        {
            await _dataContext.AddRangeAsync(Entitys);
            return true;
        }

        public bool DeletarEntity<T>(T Entity) where T : Entity
        {
            _dataContext.Remove(Entity);
            return true;
        }

        public bool RastrearEntity<T>(T Entity) where T : Entity
        {
            _dataContext.Attach(Entity);
            return true;
        }

        public bool RastrearEntitys<T>(ICollection<T> Entitys) where T : Entity
        {
            _dataContext.AttachRange(Entitys);
            return true;
        }

        public async Task<ICollection<T>> ObterTodosAsync<T>() where T : Entity
        {
            var result = await _dataContext.Set<T>().ToListAsync();

            return result;
        }
        public async Task<T?> ObterPorIdAsync<T>(int id) where T : Entity
        {
            var result = await _dataContext.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }

        public async Task<ICollection<T>> ObterPorIdsAsync<T>(ICollection<int> ids) where T : Entity
        {
            var result = await _dataContext.Set<T>().Where(x => ids.Contains(x.Id)).ToListAsync();
            return result;
        }

        public async Task<T?> ObterPorCondicaoAsync<T>(Expression<Func<T, bool>> predicate) where T : Entity
        {
            var result = await _dataContext.Set<T>().FirstOrDefaultAsync(predicate);
            return result;
        }

        public async Task<ICollection<T>> ObterTodosPorCondicaoAsync<T>(Expression<Func<T, bool>> predicate) where T : Entity
        {
            var result = await _dataContext.Set<T>().Where(predicate).ToListAsync();
            return result;
        }

        public async Task<ICollection<T>> ObterTodosPorCondicaoAsync<T>(
    Expression<Func<T, bool>> predicate,
    params Func<IQueryable<T>, IIncludableQueryable<T, object>>[] includes
) where T : Entity
        {
            IQueryable<T> query = _dataContext.Set<T>().Where(predicate);


            foreach (var include in includes)
            {
                query = include(query);
            }

            var result = await query.ToListAsync();
            return result;
        }

        public async Task<int> SalvarAlteracoesAsync()
        {
            return await _dataContext.SaveChangesAsync();
        }

        public async Task<ICollection<T>> ObterTodosAsync<T>(params Func<IQueryable<T>, IIncludableQueryable<T, object>>[] includes) where T : Entity
        {
            var query = _dataContext.Set<T>().AsQueryable();

            foreach (var include in includes)
            {
                query = include(query);
            }

            var result = await query.ToListAsync();
            return result;
        }

        public async Task<T?> ObterPorIdAsync<T>(int id, params Func<IQueryable<T>, IIncludableQueryable<T, object>>[] includes) where T : Entity
        {
            var query = _dataContext.Set<T>().AsQueryable();

            foreach (var include in includes)
            {
                query = include(query);
            }

            var result = await query.FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }

        public async Task<bool> EntityExisteAsync<T>(int id) where T : Entity
        {
            var result = await _dataContext.Set<T>().AnyAsync(x => x.Id == id);

            return result;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _dataContext.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
