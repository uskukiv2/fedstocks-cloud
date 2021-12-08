using System;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using fed.cloud.product.domain.Exceptions;
using fed.cloud.product.domain.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace fed.cloud.product.application.Commands;

public class CreateNewPurchaseLinesCommand : IRequest<Unit>
{
    public CreateNewPurchaseLinesCommand(PurchaseLineDto[] purchaseLines)
    {
        PurchaseLines = purchaseLines;
    }

    [DataMember]
    public PurchaseLineDto[] PurchaseLines { get; }
}

public class PurchaseLineDto
{
    public long Number { get; set; }

    public string Brand { get; set; }

    public string Name { get; set; }

    public decimal Price { get; set; }

    public decimal OriginalPrice { get; set; }

    public Guid Seller { get; set; }
}

public class CreateNewPurchaseLinesCommandHandler : IRequestHandler<CreateNewPurchaseLinesCommand, Unit>
{
    private readonly IMediator _mediator;
    private readonly IProductRepository _productRepository;
    private readonly ISellerCompanyRepository _sellerCompanyRepository;
    private readonly ILogger<CreateNewPurchaseLinesCommandHandler> _logger;

    public CreateNewPurchaseLinesCommandHandler(IMediator mediator, IProductRepository productRepository,
        ISellerCompanyRepository sellerCompanyRepository, ILogger<CreateNewPurchaseLinesCommandHandler> logger)
    {
        _mediator = mediator;
        _productRepository = productRepository;
        _sellerCompanyRepository = sellerCompanyRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(CreateNewPurchaseLinesCommand request, CancellationToken cancellationToken)
    {
        foreach (var purchaseLine in request.PurchaseLines)
        {
            try
            {
                var product = await _productRepository.GetByNumberAsync(purchaseLine.Number);
                if (product.Brand != purchaseLine.Brand && product.Name != purchaseLine.Name)
                {
                    _logger.LogWarning("Could not understand given purchase line with {number} {brand} {name}",
                        purchaseLine.Number, purchaseLine.Brand, purchaseLine.Name);
                    continue;
                }

                if (await _sellerCompanyRepository.IsCompanyExistsAsync(purchaseLine.Seller))
                {
                    await _productRepository.AddPurchaseForProductAsync(product.Id, purchaseLine.Price, purchaseLine.OriginalPrice, purchaseLine.Seller);
                }
            }
            catch (ProductIsNotExistException exp)
            {
                _logger.LogError(exp, "possible product with {number} isn't exists. Additional: {exmessage}", purchaseLine.Number, exp.Message);
                await _mediator.Send(new CreateNewProductCommand(purchaseLine.Number, purchaseLine.Brand,
                    purchaseLine.Name, purchaseLine.OriginalPrice, purchaseLine.Seller), cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unknown exception {number} Additional: {exmessage}", purchaseLine.Number, e.Message);
            }
        }

        return Unit.Value;
    }
}
