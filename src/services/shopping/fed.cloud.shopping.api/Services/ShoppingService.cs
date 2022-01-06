using fed.cloud.shopping.api.Application.IntegrationEvents;
using fed.cloud.shopping.api.Application.IntegrationEvents.Events;
using fed.cloud.shopping.api.Protos;
using fed.cloud.shopping.domain.Entities;
using fed.cloud.shopping.domain.Repositories;
using Grpc.Core;
using Microsoft.AspNetCore.Authentication;
using Category = fed.cloud.shopping.api.Protos.Category;
using Seller = fed.cloud.shopping.api.Protos.Seller;
using Unit = fed.cloud.shopping.domain.Entities.Unit;

namespace fed.cloud.shopping.api.Services;

public class ShoppingService : Shopping.ShoppingBase
{
    private readonly IShoppingListRepository _shoppingListRepository;
    private readonly IShoppingIntegrationEventService _shoppingIntegrationEventService;

    public ShoppingService(IShoppingListRepository shoppingListRepository, IShoppingIntegrationEventService shoppingIntegrationEventService)
    {
        _shoppingListRepository = shoppingListRepository;
        _shoppingIntegrationEventService = shoppingIntegrationEventService;
    }

    public override async Task<CheckoutResult> CheckoutList(CheckoutListRequest request, ServerCallContext context)
    {
        if (request.Id < 1)
        {
            context.Status = Status.DefaultCancelled;
            return new CheckoutResult();
        }

        if (!Guid.TryParse(request.Guid, out var userId) || userId == Guid.Empty)
        {
            context.Status = Status.DefaultCancelled;
            return new CheckoutResult();
        }

        var shoppingList = await _shoppingListRepository.GetShoppingListAsync(userId, request.Id);
        if (shoppingList == null)
        {
            context.Status = new Status(StatusCode.NotFound, "could not find list");
            return new CheckoutResult();
        }

        if (!shoppingList.Lines.All(x => x.IsChecked) && !request.IsForceCheckout)
        {
            context.Status = new Status(StatusCode.Cancelled,
                "could not perform operation, because is not all lines is done");
            return new CheckoutResult
            {
                Id = shoppingList.Id,
                Name = shoppingList.Name,
                Success = false
            };
        }

        if (!await _shoppingListRepository.DestroyListAsync(userId, request.Id))
        {
            context.Status = new Status(StatusCode.Aborted,
                "could not perform operation, database restricted operation");
            return new CheckoutResult
            {
                Id = shoppingList.Id,
                Name = shoppingList.Name,
                Success = false
            };
        }

        var transactionId = await _shoppingIntegrationEventService.BeginLocalTransaction();
        
        var currentDate = DateTime.Now.ToUniversalTime();
        var productsPurchaseEvent =
            new AddProductPurchasesEvent(currentDate, shoppingList.Lines.Select(x => MapToBoughtLine(x, shoppingList.Seller.Id)).ToArray());

        var createOrderEvent =
            new CreateOrderFromShoppingListEvent(userId, currentDate,
                shoppingList.Lines.Select(MapToOrderShoppingLine).ToArray());

        await _shoppingIntegrationEventService.CommitEventAsync(productsPurchaseEvent, transactionId);
        await _shoppingIntegrationEventService.CommitEventAsync(createOrderEvent, transactionId);

        await _shoppingIntegrationEventService.CompleteTransaction(transactionId);

        context.Status = Status.DefaultSuccess;
        return new CheckoutResult
        {
            Id = shoppingList.Id,
            Name = shoppingList.Name,
            Success = true
        };
    }

    public override async Task<ListsResponse> GetShoppingList(ListRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.UserId, out var guid) || guid == Guid.Empty)
        {
            context.Status = Status.DefaultCancelled;
            return new ListsResponse();
        }

        var shoppingLists = await _shoppingListRepository.GetShoppingListAsync(guid, request.Id);
        if (shoppingLists == null)
        {
            context.Status = new Status(StatusCode.NotFound, "list couldn't be found");
            return new ListsResponse();
        }

        var listResponse = new ListsResponse();
        listResponse.Lists.Add(MapToList(shoppingLists));

        return listResponse;
    }

    public override async Task<ListsResponse> GetShoppingLists(ListsRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.UserId, out var guid) || guid == Guid.Empty)
        {
            context.Status = Status.DefaultCancelled;
            return new ListsResponse();
        }

        var shoppingLists = await _shoppingListRepository.GetUserShoppingListsAsync(guid);
        if (shoppingLists == null || !shoppingLists.Any())
        {
            context.Status = new Status(StatusCode.NotFound, "list couldn't be found");
            return new ListsResponse();
        }

        var listResponse = new ListsResponse();
        listResponse.Lists.AddRange(shoppingLists.Select(MapToList));

        return listResponse;
    }

    public override async Task<List> CreateOrUpdate(List request, ServerCallContext context)
    {
        ShoppingList? shopping;
        if (request.Id == 0)
        {
            shopping = await _shoppingListRepository.CreateShoppingListAsync(MapToShoppingList(request));
        }
        else
        {
            shopping = await _shoppingListRepository.FullUpdateShoppingListAsync(MapToShoppingList(request));
        }

        if (shopping != null)
        {
            return MapToList(shopping);
        }

        context.Status = new Status(StatusCode.NotFound, "could not find shopping list");
        return new List();

    }

    private static List MapToList(ShoppingList shopping)
    {
        var list = new List
        {
            Id = shopping.Id,
            Guid = shopping.UserId.ToString(),
            Name = shopping.Name,
            Seller = MapToSeller(shopping.Seller)
        };

        list.Lines.AddRange(shopping.Lines.Select(MapToListLine));

        return list;
    }

    private static Seller MapToSeller(domain.Entities.Seller shoppingSeller)
    {
        return new Seller
        {
            Id = shoppingSeller.Id.ToString(),
            Name = shoppingSeller.Name,
            County = shoppingSeller.CountyId.ToString()
        };
    }

    private static ShoppingList MapToShoppingList(List request)
    {
        return new ShoppingList
        {
            UserId = Guid.Parse(request.Guid),
            Id = request.Id,
            Name = request.Name,
            Seller = new domain.Entities.Seller
            {
                Id = Guid.Parse(request.Seller.Id),
                Name = request.Seller.Name,
                CountyId = Guid.Parse(request.Seller.County)
            },
            Lines = request.Lines.Select(MapToListLine)
        };
    }

    private static ShoppingListLine MapToListLine(Line arg)
    {
        return new ShoppingListLine
        {
            Id = Guid.NewGuid(),
            IsChecked = arg.Checked,
            Quantity = arg.Quantity,
            UnitPrice = (decimal) arg.UnitPrice,
            Unit = MapUnit(arg.Unit),
            ProductName = arg.Name,
            ProductNumber = arg.Number,
            ProductBrand = arg.Brand,
            Category = MapToCategory(arg.Category)
        };
    }

    private static Line MapToListLine(ShoppingListLine listLine)
    {
        return new Line
        {
            Brand = listLine.ProductBrand,
            Name = listLine.ProductName,
            Number = listLine.ProductNumber,
            Checked = listLine.IsChecked,
            Unit = MapToUnit(listLine.Unit),
            UnitPrice = decimal.ToDouble(listLine.UnitPrice),
            Quantity = listLine.Quantity,
            Category = MapToCategory(listLine.Category)
        };
    }

    private static Category MapToCategory(domain.Entities.Category category)
    {
        return new Category
        {
            Id = category.Id,
            Name = category.Name,
            Parent = category.Parent != null ? MapToCategory(category.Parent) : null
        };
    }
    
    private static domain.Entities.Category MapToCategory(Category argCategory)
    {
        return new domain.Entities.Category
        {
            Id = argCategory.Id,
            Name = argCategory.Name,
            Parent = argCategory.Parent != null ? MapToCategory(argCategory.Parent) : null
        };
    }

    private static Protos.Unit MapToUnit(Unit listLineUnit)
    {
        return new Protos.Unit
        {
            Id = listLineUnit.Id,
            Name = listLineUnit.Name,
            Rate = listLineUnit.Rate
        };
    }

    private static Unit MapUnit(Protos.Unit argUnit)
    {
        return new Unit
        {
            Id = argUnit.Id,
            Name = argUnit.Name,
            Rate = argUnit.Rate
        };
    }

    private static BoughtProduct MapToBoughtLine(ShoppingListLine arg, Guid sellerId)
    {
        return new BoughtProduct
        {
            Brand = arg.ProductBrand,
            Name = arg.ProductName,
            Number = arg.ProductNumber,
            OriginalPrice = arg.UnitPrice,
            Seller = sellerId,
            UnitId = arg.Unit.Id,
            CategoryId = arg.Category.Id
        };
    }

    private static OrderLine MapToOrderShoppingLine(ShoppingListLine arg)
    {
        return new OrderLine
        {
            Name = $"({arg.ProductBrand}) {arg.ProductName}",
            Number = arg.ProductNumber,
            Quantity = arg.Quantity,
            Unit = MapToUnit(arg.Unit),
            UnitPrice = arg.UnitPrice
        };
    }
}