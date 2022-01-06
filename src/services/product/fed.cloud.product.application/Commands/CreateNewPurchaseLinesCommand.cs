using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using fed.cloud.product.application.Models;
using fed.cloud.product.domain.Abstraction;
using fed.cloud.product.domain.Exceptions;
using fed.cloud.product.domain.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace fed.cloud.product.application.Commands;

public class CreateNewPurchaseLinesCommand : INotification
{
    public CreateNewPurchaseLinesCommand(PurchaseLineDto[] purchaseLines)
    {
        PurchaseLines = purchaseLines;
    }

    [DataMember] public PurchaseLineDto[] PurchaseLines { get; }
}

public class CreateNewPurchaseLinesCommandHandler : INotificationHandler<CreateNewPurchaseLinesCommand>
{
    private readonly IMediator _mediator;
    private readonly IUnitOfWork<NpgsqlConnection> _unitOfWork;
    private readonly IProductRepository _productRepository;
    private readonly ISellerCompanyRepository _sellerCompanyRepository;
    private readonly ILogger<CreateNewPurchaseLinesCommandHandler> _logger;

    public CreateNewPurchaseLinesCommandHandler(
        IMediator mediator,
        IUnitOfWork<NpgsqlConnection> unitOfWork,
        IProductRepository productRepository,
        ISellerCompanyRepository sellerCompanyRepository,
        ILogger<CreateNewPurchaseLinesCommandHandler> logger)
    {
        _mediator = mediator;
        _unitOfWork = unitOfWork;
        _productRepository = productRepository;
        _sellerCompanyRepository = sellerCompanyRepository;
        _logger = logger;
    }

    public async Task Handle(CreateNewPurchaseLinesCommand request, CancellationToken cancellationToken)
    {
        using (_logger.BeginScope("begin adding purchase lines"))
        {
            var productsToCreate = new List<PurchaseLineDto>();
            try
            {
                await _unitOfWork.BeginAsync();

                try
                {
                    foreach (var purchaseLine in request.PurchaseLines)
                    {
                        try
                        {
                            var product = await _productRepository.GetByNumberAsync(purchaseLine.Number);
                            if (product.Brand != purchaseLine.Brand && product.Name != purchaseLine.Name)
                            {
                                _logger.LogWarning(
                                    "Could not understand given purchase line with {number} {brand} {name}",
                                    purchaseLine.Number, purchaseLine.Brand, purchaseLine.Name);
                                continue;
                            }

                            if (await _sellerCompanyRepository.IsCompanyExistsAsync(purchaseLine.Seller))
                            {
                                await _productRepository.AddPurchaseForProductAsync(product.Id, purchaseLine.Price,
                                    purchaseLine.OriginalPrice, purchaseLine.Seller);
                            }
                        }
                        catch (ProductIsNotExistException exp)
                        {
                            _logger.LogError(exp,
                                "possible product with {number} isn't exists. Additional: {exmessage}",
                                purchaseLine.Number, exp.Message);
                            productsToCreate.Add(purchaseLine);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, "Unknown exception {number} Additional: {exmessage}",
                                purchaseLine.Number, e.Message);
                        }
                    }

                    await _unitOfWork.CommitAsync();
                }
                catch (TransactionException ex)
                {
                    _logger.LogError(ex, "transaction error happen");
                    await _unitOfWork.RollbackAsync();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to create transaction");
                _unitOfWork.DropTransaction();
            }
            
            _logger.LogTrace($"trying to create new products");
            await _mediator.Send(new CreateNewProductCommand(productsToCreate), cancellationToken);
        }
    }
}