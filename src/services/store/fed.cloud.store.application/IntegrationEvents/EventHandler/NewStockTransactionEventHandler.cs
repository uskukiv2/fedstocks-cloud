using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using fed.cloud.eventbus.Base;
using fed.cloud.store.application.IntegrationEvents.Events;
using fed.cloud.store.domain.Extras;
using fed.cloud.store.domain.Root.Stock;
using Microsoft.Extensions.Logging;

namespace fed.cloud.store.application.IntegrationEvents.EventHandler;

public class StockTransactionRemoveItemEventHandler : IIntegrationEventHandler<StockTransactionRemoveItemEvent>
{
    private readonly ILogger<StockTransactionRemoveItemEventHandler> _logger;
    private readonly IStockRepository _stockRepository;

    public StockTransactionRemoveItemEventHandler(IStockRepository stockRepository,
        ILogger<StockTransactionRemoveItemEventHandler> logger)
    {
        _stockRepository = stockRepository;
        _logger = logger;
    }

    public async Task Handle(StockTransactionRemoveItemEvent @event)
    {
        _logger.LogInformation($"Received event {@event.Id} {@event.GetType().Name} created at {@event.CreationDate}");
        try
        {
            var stock = await _stockRepository.GetAsync(@event.StockId);
            if (stock is null)
            {
                _logger.LogWarning($"---- Stock cannot be found with {@event.StockId}. Stopping event handling ----");
                return;
            }


            if (stock.StockItems.All(x => x.Number != @event.ProductNumber))
            {
                _logger.LogWarning(
                    $"---- {@event.Id} {@event.GetType().Name} Stock item with number {@event.ProductNumber} isn't exist. Skip event handler execution");
                return;
            }

            var stockItem = stock.StockItems.FirstOrDefault(x => x.Number == @event.ProductNumber)!;
            stock.RemoveStockItem(stockItem.Id);

            await _stockRepository.RemoveStockItems(stock.Id, new[] {stockItem}, CancellationToken.None);
            _logger.LogInformation(
                $"Event complete {@event.Id} {@event.GetType().Name} completed at: {DateTime.UtcNow}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"Caught an error while event handling {@event.Id} {@event.GetType().Name} \t {ex.Message}");
        }
    }
}

public class StockTransactionUpdateItemEventHandler : IIntegrationEventHandler<StockTransactionUpdateItemEvent>
{
    private readonly ILogger<StockTransactionUpdateItemEventHandler> _logger;
    private readonly IStockRepository _stockRepository;

    public StockTransactionUpdateItemEventHandler(IStockRepository stockRepository,
        ILogger<StockTransactionUpdateItemEventHandler> logger)
    {
        _stockRepository = stockRepository;
        _logger = logger;
    }

    public async Task Handle(StockTransactionUpdateItemEvent @event)
    {
        _logger.LogInformation($"Received event {@event.Id} {@event.GetType().Name} created at {@event.CreationDate}");
        try
        {
            var stock = await _stockRepository.GetAsync(@event.StockId);
            if (stock is null)
            {
                _logger.LogWarning(
                    $"---- {@event.Id} {@event.GetType().Name}  Stock cannot be found with {@event.StockId}. Skip event handling ----");
                return;
            }

            if (stock.StockItems.All(x => x.Number != @event.ProductNumber))
            {
                _logger.LogWarning(
                    $"---- {@event.Id} {@event.GetType().Name} Stock item with number {@event.ProductNumber} isn't exist. Skip event handler execution");
                return;
            }

            var stockItem = stock.StockItems.FirstOrDefault(x => x.Number == @event.ProductNumber);
            if (stockItem is null)
            {
                _logger.LogError(
                    $"---- {@event.Id} {@event.GetType().Name} Stock item is not exist in stock {stock.Id}");
                return;
            }

            var result = stockItem.Quantity += @event.Quantity;
            if (result < 0)
                stockItem.Quantity = 0;
            else
                stockItem.Quantity = result;

            _stockRepository.Update(stock);
            _logger.LogInformation(
                $"Event complete {@event.Id} {@event.GetType().Name} completed at: {DateTime.UtcNow}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"Caught an error while event handling {@event.Id} {@event.GetType().Name} \t {ex.Message}");
        }
    }
}

public class StockTransactionAddItemEventHandler : IIntegrationEventHandler<StockTransactionAddItemEvent>
{
    private readonly ILogger<StockTransactionAddItemEventHandler> _logger;
    private readonly IStockRepository _stockRepository;

    public StockTransactionAddItemEventHandler(IStockRepository stockRepository,
        ILogger<StockTransactionAddItemEventHandler> logger)
    {
        _stockRepository = stockRepository;
        _logger = logger;
    }

    public async Task Handle(StockTransactionAddItemEvent @event)
    {
        _logger.LogInformation($"Received event {@event.Id} {@event.GetType().Name} created at {@event.CreationDate}");

        try
        {
            var stock = await _stockRepository.GetAsync(@event.StockId);
            if (stock is null)
            {
                _logger.LogWarning($"---- Stock cannot be found with {@event.StockId}. Stopping event handling ----");
                return;
            }

            if (stock.StockItems.Any(x => x.Number == @event.ProductNumber))
            {
                _logger.LogWarning(
                    $"---- Stock item with number {@event.ProductNumber} is already exist. Skip event handler execution");
                return;
            }

            stock.AddStockItem(@event.ProductNumber,
                @event.ProductName,
                @event.CategoryId,
                (UnitType) @event.UnitId,
                @event.Quantity);

            _stockRepository.Update(stock);
            _logger.LogInformation(
                $"Event complete {@event.Id} {@event.GetType().Name} completed at: {DateTime.UtcNow}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"Caught an error while event handling {@event.Id} {@event.GetType().Name} \t {ex.Message}");
        }
    }
}