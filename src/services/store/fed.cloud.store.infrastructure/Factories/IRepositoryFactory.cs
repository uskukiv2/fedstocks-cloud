using fed.cloud.common.Infrastructure;
using RepoDb;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace fed.cloud.store.infrastructure.Factories
{
    public interface IRepositoryFactory<TDbConnection> where TDbConnection : DbConnection, new()
    {
        IPortableRepository<TEntity> Create<TEntity>() where TEntity : class;
    }

    public class RepositoryFactoryImpl<TDbConnection> : IRepositoryFactory<TDbConnection>
        where TDbConnection : DbConnection, new()
    {
        private static readonly object Lock = new object();

        private readonly IServiceConfiguration _serviceConfiguration;
        private readonly IUnitOfWork _unitOfWork;

        public RepositoryFactoryImpl(IServiceConfiguration serviceConfiguration, IUnitOfWork unitOfWork)
        {
            _serviceConfiguration = serviceConfiguration;
            _unitOfWork = unitOfWork;
        }

        public IPortableRepository<TEntity> Create<TEntity>() where TEntity : class
        {
            lock (Lock)
            {
                return new PortableRepository<TEntity, TDbConnection>(_serviceConfiguration.Database.Connection,
                    _unitOfWork);
            }
        }
    }

    public interface IPortableRepository<TEntity>
    {
        Task<IEnumerable<TEntity>> QueryAsync<TWhat>(string tableName, TWhat what, CancellationToken token);
        Task<int> DeleteAllAsync<TKey>(IEnumerable<TKey> entities, CancellationToken token);
        Task<int> InsertAllAsync(IEnumerable<TEntity> entities, CancellationToken token);
        Task<int> UpdateAllAsync(IEnumerable<TEntity> entities, CancellationToken token);
    }

    public class PortableRepository<TEntity, TDbConnection> : BaseRepository<TEntity, TDbConnection>, IPortableRepository<TEntity>
        where TDbConnection : DbConnection, new() where TEntity : class
    {
        private readonly IUnitOfWork _unitOfWork;

        public PortableRepository(string connectionString, IUnitOfWork unitOfWork) : base(connectionString)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<int> DeleteAllAsync<TKey>(IEnumerable<TKey> entities, CancellationToken token)
        {
            return DeleteAllAsync(entities, transaction: _unitOfWork.Transaction, cancellationToken: token);
        }

        public Task<int> InsertAllAsync(IEnumerable<TEntity> entities, CancellationToken token)
        {
            return InsertAllAsync(entities, transaction: _unitOfWork.Transaction, cancellationToken: token);
        }

        public Task<IEnumerable<TEntity>> QueryAsync<TWhat>(string tableName, TWhat what, CancellationToken token)
        {
            return QueryAsync(tableName, what, cancellationToken: token);
        }

        public Task<int> UpdateAllAsync(IEnumerable<TEntity> entities, CancellationToken token)
        {
            return UpdateAllAsync(entities, transaction: _unitOfWork.Transaction, cancellationToken: token);
        }
    }
}
