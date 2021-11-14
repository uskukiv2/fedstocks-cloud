using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using fed.cloud.common.Infrastructure;
using fed.cloud.store.domain.Abstract;
using fed.cloud.store.domain.Root.Order;
using fed.cloud.store.domain.Root.Stock;
using fed.cloud.store.infrastructure.Factories;
using Npgsql;
using RepoDb;
using RepoDb.Enumerations;

namespace fed.cloud.store.infrastructure.Repositories
{
    public class StockRepository : BaseRepository<Stock, NpgsqlConnection>, IStockRepository
    {
        private readonly IPortableRepository<StockItem> _stockItemRepository;

        public StockRepository(IServiceConfiguration configuration, IUnitOfWork<NpgsqlConnection> unitOfWork,
            IRepositoryFactory<NpgsqlConnection> repositoryFactory)
            : base(configuration.Database.Connection)
        {
            UnitOfWork = unitOfWork;
            _stockItemRepository = repositoryFactory.Create<StockItem>();
        }

        public IUnitOfWork UnitOfWork { get; }

        public async Task<Stock> GetAsync(Guid id)
        {
            var fields = new QueryField("Id", id);
            var stock = (await QueryAsync(fields)).FirstOrDefault()!;

            return stock;
        }

        public void Add(Stock entity, CancellationToken token = default)
        {
            InsertAsync(entity, transaction: UnitOfWork.Transaction, cancellationToken: token);
            _stockItemRepository.InsertAllAsync(entity.StockItems, token);
        }

        public void Update(Stock entity, CancellationToken token = default)
        {
            UpdateAsync(entity, transaction: UnitOfWork.Transaction, cancellationToken: token);
            _stockItemRepository.UpdateAllAsync(entity.StockItems, token);
        }

        public async Task<Stock> GetDefaultStockAsync(Guid userId)
        {
            var group = new QueryGroup(new List<QueryField>()
            {
                new QueryField("UserId", userId),
                new QueryField("IsDefault", true)
            });

            var stock = (await QueryAsync(group)).FirstOrDefault()!;

            return stock;
        }

        public async Task<IEnumerable<Stock>> GetStocksAsync(Guid userId)
        {
            var fields = new QueryField("UserId", userId);
            var stock = await QueryAsync(fields);

            return stock;
        }

        public async Task<bool> IsStockExist(Guid stockId)
        {
            var fields = new QueryField("Id", stockId);
            var isExists = await ExistsAsync(fields);

            return isExists;
        }

        public async Task<Stock> GetGroupStockAsync(Guid groupId)
        {
            var fields = new QueryField("GroupId", groupId);
            var stock = (await QueryAsync(fields)).FirstOrDefault()!;

            return stock;
        }

        public async Task<int> RemoveStockItems(Guid stockId, IEnumerable<StockItem> stockItemsToRemove, CancellationToken token = default)
        {
            return await _stockItemRepository.DeleteAllAsync(stockItemsToRemove, token);
        }
    }
}