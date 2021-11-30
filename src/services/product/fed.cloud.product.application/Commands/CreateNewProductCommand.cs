using System;
using System.Threading;
using System.Threading.Tasks;
using fed.cloud.product.domain.Entities;
using fed.cloud.product.domain.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace fed.cloud.product.application.Commands;

public class CreateNewProductCommand : IRequest<Unit>
{
    public CreateNewProductCommand(long number, string brand, string name, decimal originalPrice, Guid seller)
    {
        Number = number;
        Brand = brand;
        Name = name;
        OriginalPrice = originalPrice;
        Seller = seller;
    }

    public long Number { get; }

    public string Brand { get; }

    public string Name { get; }

    public  decimal OriginalPrice { get; }

    public Guid Seller { get; }
}

public class CreateNewProductCommandHandler : IRequestHandler<CreateNewProductCommand, Unit>
{
    private readonly IProductRepository _productRepository;
    private readonly ISellerCompanyRepository _sellerCompanyRepository;
    private readonly ILogger<CreateNewProductCommandHandler> _logger;

    public CreateNewProductCommandHandler(IProductRepository productRepository, ISellerCompanyRepository sellerCompanyRepository, ILogger<CreateNewProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _sellerCompanyRepository = sellerCompanyRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(CreateNewProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = new Product
            {
                Brand = request.Brand,
                Name = request.Name,
                GlobalNumber = request.Number
            };
            _productRepository.Add(product, cancellationToken);

            var originalProduct = await _productRepository.GetByNumberAsync(request.Number);
            if (await _sellerCompanyRepository.IsCompanyExistsAsync(request.Seller))
            {
                await _productRepository.AddPurchaseForProductAsync(originalProduct.Id, decimal.Zero,
                    request.OriginalPrice, request.Seller);
            }

            return Unit.Value;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return Unit.Value;
        }
    }
}
