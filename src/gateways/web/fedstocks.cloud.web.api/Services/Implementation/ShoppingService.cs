﻿using fed.cloud.communication.Shopper;
using fed.cloud.shopping.api.Protos;
using fedstocks.cloud.web.api.Helpers;
using Category = fed.cloud.shopping.api.Protos.Category;
using Seller = fed.cloud.shopping.api.Protos.Seller;
using Unit = fed.cloud.shopping.api.Protos.Unit;

namespace fedstocks.cloud.web.api.Services.Implementation;

public class ShoppingService : IShoppingService
{
    private readonly Shopping.ShoppingClient _shoppingClient;
    private readonly ILogger<ShoppingService> _logger;

    public ShoppingService(Shopping.ShoppingClient shoppingClient, ILogger<ShoppingService> logger)
    {
        _shoppingClient = shoppingClient;
        _logger = logger;
    }

    public async Task<ShoppingCheckoutResult> CheckoutShoppingListAsync(Guid userId, int shoppingListId)
    {
        _logger.LogTrace("sending request to shopping - checkout");
        var request = new CheckoutListRequest
        {
            Id = shoppingListId,
            Guid = userId.ToString(),
            IsForceCheckout = false
        };
        try
        {
            var response = await _shoppingClient.CheckoutListAsync(request, deadline: DateTime.UtcNow.AddSeconds(10));
            if (response == null)
            {
                _logger.LogDebug("checkout - respond with null");
                return new ShoppingCheckoutResult
                {
                    Name = string.Empty,
                    ShoppingId = shoppingListId,
                    TotalLines = 0
                };
            }

            if (!response.Success)
            {
                return new ShoppingCheckoutResult()
                {
                    Name = response.Name,
                    ShoppingId = response.Id,
                    TotalLines = -1
                };
            }

            _logger.LogTrace("checkout - respond with {0} {1} is done {2}", response.Id, response.Name, response.Success);
            return MapToDto(response);
        }
        catch (Exception ex)
        {
            return ExceptionHelper.HandleExceptionWithGrpc<ShoppingCheckoutResult>(_logger, ex);
        }
    }

    public async Task<CompletedShoppingList> CreateShoppingListAsync(NewShoppingList newList, Guid userId)
    {
        _logger.LogTrace("sending request to shopping - new list");
        var request = new List
        {
            Id = 0,
            Guid = userId.ToString(),
            Name = newList.Name,
            Seller = new Seller
            {
                Id = newList.Seller.Id.ToString(),
                Name = newList.Seller.Name,
                County = newList.Seller.CountyId.ToString()
            }
        };
        newList.Lines.ToList().ForEach(x => request.Lines.Add(MapToRequest(x)));

        try
        {
            var response = await _shoppingClient.CreateOrUpdateAsync(request, deadline: DateTime.UtcNow.AddSeconds(10));
            if (response == null)
            {
                _logger.LogDebug("create list - respond with null");
                return new CompletedShoppingList
                {
                    Id = 0,
                    Name = "undefined",
                    Lines = new List<ShoppingListLine>()
                };
            }

            _logger.LogTrace("create list - respond with {0} {1}", response.Id, response.Name);
            return MapToDto(response);
        }
        catch (Exception ex)
        {
            return ExceptionHelper.HandleExceptionWithGrpc<CompletedShoppingList>(_logger, ex);
        }
    }

    // TODO: FED-124
    public Task<bool> DeleteShoppingListAsync(Guid userId, int shoppingListId)
    {
        return Task.FromResult(false);
    }

    public async Task<CompletedShoppingList> GetShoppingListAsync(Guid userId, int listId)
    {
        _logger.LogTrace("sending request to shopping - get list {0}", listId);
        if (listId < 1)
        {
            _logger.LogDebug("Cannot get list with zero id");
            return new CompletedShoppingList();
        }

        var request = new ListRequest
        {
            UserId = userId.ToString(),
            Id = listId
        };

        try
        {

            var result = await _shoppingClient.GetShoppingListAsync(request, deadline: DateTime.UtcNow.AddSeconds(10));
            if (result != null && result.Lists.Any() && result.Lists.Count == 1)
            {
                _logger.LogTrace("get list - respond with count {0}", result.Lists.Count);
                return MapToDto(result.Lists.FirstOrDefault());
            }

            _logger.LogDebug("received unexpected list count {0}", result?.Lists.Count);
            return new CompletedShoppingList();
        }
        catch (Exception ex)
        {
            return ExceptionHelper.HandleExceptionWithGrpc<CompletedShoppingList>(_logger, ex);
        }
    }

    public async Task<IEnumerable<CompletedShoppingList>> GetShoppingListsAsync(Guid userId)
    {
        _logger.LogTrace("sending request to shopping - get list");
        var request = new ListsRequest
        {
            UserId = userId.ToString()
        };
        try
        {
            var result = await _shoppingClient.GetShoppingListsAsync(request);
            if (result != null && result.Lists.Any() && result.Lists.Count > 0)
            {
                _logger.LogTrace("get list - respond with count {0}", result.Lists.Count);
                return result.Lists.Select(MapToDto);
            }

            _logger.LogDebug("received unexpected list count {0}", result?.Lists.Count);
            return new List<CompletedShoppingList>();
        }
        catch (Exception ex)
        {
            return ExceptionHelper.HandleExceptionWithGrpc<List<CompletedShoppingList>>(_logger, ex);
        }
    }

    public async Task<CompletedShoppingList> UpdateShoppingListAsync(CompletedShoppingList list, Guid userId)
    {
        _logger.LogTrace("sending request to shopping - get list");
        var request = MapToRequest(list, userId);
        try
        {
            var result = await _shoppingClient.CreateOrUpdateAsync(request, deadline: DateTime.UtcNow.AddSeconds(10));
            if (result != null)
            {
                _logger.LogTrace("update list - respond with {0} {1}", result.Id, result.Name);
                return MapToDto(result);
            }

            _logger.LogDebug("update list - respond with null");
            return new CompletedShoppingList();
        }
        catch (Exception ex)
        {
            return ExceptionHelper.HandleExceptionWithGrpc<CompletedShoppingList>(_logger, ex);
        }
    }

    private static ShoppingCheckoutResult MapToDto(CheckoutResult response)
    {
        return new ShoppingCheckoutResult
        {
            ShoppingId = response.Id,
            Name = response.Name,
            TotalLines = 1
        };
    }

    private static List MapToRequest(CompletedShoppingList shoppingList, Guid userId)
    {
        var list = new List
        {
            Id = shoppingList.Id,
            Name = shoppingList.Name,
            Guid = userId.ToString(),
            Seller = new Seller
            {
                Id = shoppingList.Seller.Id.ToString(),
                Name = shoppingList.Seller.Name,
                County = shoppingList.Seller.CountyId.ToString()
            }
        };
        shoppingList.Lines.ToList().ForEach(x => list.Lines.Add(MapToRequest(x)));

        return list;
    }

    private static Line MapToRequest(ShoppingListLine line)
    {
        return new Line
        {
            Name = line.ProductName,
            Brand = line.ProductBrand,
            Checked = line.IsChecked,
            Number = line.ProductNumber,
            Quantity = line.Quantity,
            UnitPrice = decimal.ToDouble(line.UnitPrice),
            Unit = MapToRequest(line.Unit),
            Category = MapToRequest(line.Category)
        };
    }

    private static Category MapToRequest(fed.cloud.communication.Category lineCategory)
    {
        return new Category
        {
            Id = lineCategory.Id,
            Name = lineCategory.Name,
            Parent = lineCategory.ParentCategory != null ? MapToRequest(lineCategory.ParentCategory) : null
        };
    }

    private static Unit MapToRequest(fed.cloud.communication.Unit unit)
    {
        return new Unit
        {
            Id = unit.Id,
            Name = unit.Name,
            Rate = unit.Rate
        };
    }

    private static CompletedShoppingList MapToDto(List? response)
    {
        return new CompletedShoppingList
        {
            Id = response.Id,
            Name = response.Name,
            Seller = new fed.cloud.communication.Seller.Seller
            {
                Id = Guid.Parse(response.Seller.Id),
                Name = response.Seller.Name,
                CountyId = Guid.Parse(response.Seller.County)
            },
            Lines = response.Lines.Select(MapToDto)
        };
    }

    private static ShoppingListLine MapToDto(Line line)
    {
        return new ShoppingListLine
        {
            ProductNumber = line.Number,
            ProductBrand = line.Brand,
            ProductName = line.Name,
            IsChecked = line.Checked,
            Quantity = line.Quantity,
            UnitPrice = (decimal)line.UnitPrice,
            Unit = MapToDto(line.Unit),
            Category = MapToDto(line.Category)
        };
    }

    private static fed.cloud.communication.Category MapToDto(Category lineCategory)
    {
        return new fed.cloud.communication.Category
        {
            Id = lineCategory.Id,
            Name = lineCategory.Name,
            ParentCategory = lineCategory.Parent is not null ? MapToDto(lineCategory.Parent) : null
        };
    }

    private static fed.cloud.communication.Unit MapToDto(Unit unit)
    {
        return new fed.cloud.communication.Unit
        {
            Id = unit.Id,
            Name = unit.Name,
            Rate = unit.Rate,
        };
    }
}