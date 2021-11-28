using fed.cloud.product.application.Models;
using fed.cloud.product.domain.Entities;
using fed.cloud.product.domain.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace fed.cloud.product.application.Commands
{
    public class HandleProductsRequestQueryCommand : IRequest<IEnumerable<ProductSummaryDto>>
    {
        public HandleProductsRequestQueryCommand(string inputString)
        {
            Query = inputString;
        }

        public string Query { get; }
    }

    public class HandleProductsRequestQueryCommandHandler : IRequestHandler<HandleProductsRequestQueryCommand, IEnumerable<ProductSummaryDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<HandleProductsRequestQueryCommandHandler> _logger;

        public HandleProductsRequestQueryCommandHandler(IProductRepository productRepository, ILogger<HandleProductsRequestQueryCommandHandler> logger)
        {
            _productRepository=productRepository;
            _logger=logger;
        }

        public async Task<IEnumerable<ProductSummaryDto>> Handle(HandleProductsRequestQueryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _productRepository.TTSearchAsync(request.Query, cancellationToken);

                return result.Select(x => MapToDto(x));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cought error while getting products with {string}", request.Query);
                return new List<ProductSummaryDto>();
            }
        }

        private static ProductSummaryDto MapToDto(Product arg1)
        {
            return new ProductSummaryDto
            {
                Brand = arg1.Brand,
                Title = arg1.Name,
                Number = arg1.GlobalNumber
            };
        }
    }
}