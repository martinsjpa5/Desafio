using System.Data;
using Dapper;
using Domain.Interfaces.Repositories;
using Microsoft.Data.SqlClient;

namespace Infraestrutura.Dapper
{
    public class CommonRepositoryDapper : IDisposable, ICommonRepositoryDapper
    {
        private readonly IDbConnection _connection;
        private IDbTransaction _transaction;

        public CommonRepositoryDapper(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
            _connection.Open();
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null)
        {
            return await _connection.QueryAsync<T>(sql, parameters, _transaction);
        }

        public async Task<T?> QuerySingleAsync<T>(string sql, object parameters = null)
        {
            return await _connection.QuerySingleOrDefaultAsync<T>(sql, parameters, _transaction);
        }

        public async Task<int> ExecuteAsync(string sql, object parameters = null)
        {
            return await _connection.ExecuteAsync(sql, parameters, _transaction);
        }

        public void BeginTransaction()
        {
            _transaction = _connection.BeginTransaction();
        }

        public void CommitTransaction()
        {
            _transaction?.Commit();
            _transaction = null;
        }

        public void RollbackTransaction()
        {
            _transaction?.Rollback();
            _transaction = null;
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
            }

            if (_connection != null && _connection.State == ConnectionState.Open)
            {
                _connection.Close();
                _connection.Dispose();
            }
        }

    }
}
