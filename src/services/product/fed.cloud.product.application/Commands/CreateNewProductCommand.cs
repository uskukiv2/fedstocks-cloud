using fed.cloud.product.application.Models;
using fed.cloud.product.domain.Abstraction;
using fed.cloud.product.domain.Entities;
using fed.cloud.product.domain.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace fed.cloud.product.application.Commands;

public class CreateNewProductCommand : IRequest
{
    public CreateNewProductCommand(IEnumerable<PurchaseLineDto> purchaseLineDtos)
    {
        PurchaseLineDtos = purchaseLineDtos;
    }

    public IEnumerable<PurchaseLineDto> PurchaseLineDtos { get; }
}

public class CreateNewProductCommandHandler : IRequestHandler<CreateNewProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork<NpgsqlConnection> _unitOfWork;
    private readonly ISellerCompanyRepository _sellerCompanyRepository;
    private readonly ILogger<CreateNewProductCommandHandler> _logger;

    public CreateNewProductCommandHandler(IProductRepository productRepository, IUnitOfWork<NpgsqlConnection> unitOfWork, ISellerCompanyRepository sellerCompanyRepository, ILogger<CreateNewProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _sellerCompanyRepository = sellerCompanyRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(CreateNewProductCommand request, CancellationToken cancellationToken)
    {
        foreach (var lineDto in request.PurchaseLineDtos)
        {
            try
            {
                var product = new Product
                {
                    Brand = lineDto.Brand,
                    Name = lineDto.Name,
                    GlobalNumber = lineDto.Number,
                    UnitId = lineDto.UnitId,
                    CategoryId = lineDto.CategoryId
                };
                _productRepository.Add(product, cancellationToken);

                await _unitOfWork.CommitAsync();
                await _unitOfWork.BeginAsync();

                var originalProduct = await _productRepository.GetByNumberAsync(lineDto.Number);
                if (await _sellerCompanyRepository.IsCompanyExistsAsync(lineDto.Seller))
                {
                    await _productRepository.AddPurchaseForProductAsync(originalProduct.Id, decimal.Zero,
                        lineDto.OriginalPrice, lineDto.Seller);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }
        }

        return Unit.Value;
    }
}
