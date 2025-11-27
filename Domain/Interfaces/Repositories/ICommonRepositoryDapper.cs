
namespace Domain.Interfaces.Repositories
{
    public interface ICommonRepositoryDapper
    {
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null);
        Task<T?> QuerySingleAsync<T>(string sql, object parameters = null);
        Task<int> ExecuteAsync(string sql, object parameters = null);
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
    }
}
