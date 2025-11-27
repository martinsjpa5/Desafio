using global::Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infraestrutura.EF.Interfaces
{
    public interface ICommonEfRepository
    {
        bool RastrearEntity<T>(T Entity) where T : Entity;
        Task<bool> AdicionarEntityAsync<T>(T Entity) where T : Entity;
        Task<bool> AdicionarEntitysAsync<T>(List<T> Entitys) where T : Entity;
        bool RastrearEntitys<T>(ICollection<T> Entitys) where T : Entity;

        bool DeletarEntity<T>(T Entity) where T : Entity;
        Task<ICollection<T>> ObterTodosAsync<T>() where T : Entity;
        Task<T?> ObterPorIdAsync<T>(int id) where T : Entity;
        Task<ICollection<T>> ObterPorIdsAsync<T>(ICollection<int> ids) where T : Entity;
        Task<T?> ObterPorCondicaoAsync<T>(Expression<Func<T, bool>> predicate) where T : Entity;
        Task<ICollection<T>> ObterTodosPorCondicaoAsync<T>(Expression<Func<T, bool>> predicate) where T : Entity;
        Task<ICollection<T>> ObterTodosPorCondicaoAsync<T>(Expression<Func<T, bool>> predicate,
            params Func<IQueryable<T>, IIncludableQueryable<T, object>>[] includes
        ) where T : Entity;

        Task<bool> EntityExisteAsync<T>(int id) where T : Entity;
        Task<int> SalvarAlteracoesAsync();
        Task<ICollection<T>> ObterTodosAsync<T>(params Func<IQueryable<T>, IIncludableQueryable<T, object>>[] includes) where T : Entity;
        Task<T?> ObterPorIdAsync<T>(int id, params Func<IQueryable<T>, IIncludableQueryable<T, object>>[] includes) where T : Entity;
    }
}
