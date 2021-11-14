using System;
using System.Data.Common;
using System.Threading.Tasks;
using fed.cloud.common.Infrastructure;
using fed.cloud.store.domain.Abstract;
using Microsoft.Extensions.Options;
using Npgsql;

namespace fed.cloud.store.infrastructure
{
    public class PostgresUnitOfWork : IUnitOfWork<NpgsqlConnection>
    {
        private readonly IServiceConfiguration _configuration;

#pragma warning disable CS8618
        public PostgresUnitOfWork(IServiceConfiguration configuration)
#pragma warning restore CS8618
        {
            _configuration = configuration;
        }

        public NpgsqlConnection Connection { get; private set; }

        public DbTransaction? Transaction { get; private set; }

        public Task BeginAsync()
        {
            if (Transaction != null)
            {
                return Task.FromException(new InvalidOperationException("Cannot create a new transaction wit existing one. Transaction already started"));
            }

            var connection = EnsureConnection();
            Transaction = connection.BeginTransaction();
            return Task.CompletedTask;
        }

        public Task CommitAsync()
        {
            if (Transaction == null)
            {
                return Task.FromException(new InvalidOperationException("There is no active transaction to commit."));
            }
            using (Transaction)
            {
                Transaction.Commit();
            }
            Transaction = null;
            return Task.CompletedTask;
        }

        public Task RollbackAsync()
        {
            if (Transaction == null)
            {
                return Task.FromException(new InvalidOperationException("There is no active transaction to rollback."));
            }
            using (Transaction)
            {
                Transaction.Rollback();
            }
            Transaction = null;

            return Task.CompletedTask;
        }

        private NpgsqlConnection EnsureConnection()
        {
            return Connection ??= new NpgsqlConnection(_configuration.Database.Connection);
        }
    }
}
