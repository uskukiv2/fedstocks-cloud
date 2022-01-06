using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fed.cloud.eventbus.Base;
using fed.cloud.product.application.Commands;
using fed.cloud.product.application.IntegrationEvents.Events;
using fed.cloud.product.application.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace fed.cloud.product.application.IntegrationEvents.Handlers
{
    public class AddProductPurchasesEventHandler : IIntegrationEventHandler<AddProductPurchasesEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AddProductPurchasesEventHandler> _logger;

        public AddProductPurchasesEventHandler(IMediator mediator, ILogger<AddProductPurchasesEventHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Handle(AddProductPurchasesEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-Products"))
            {
                try
                {
                    _logger.LogInformation("---- Handle bought products originalDate: {eventDate}; {event}",
                        @event.BoughtDate, @event);

                    var command = new CreateNewPurchaseLinesCommand(MapToDto(@event.Lines).ToArray());
                    await _mediator.Publish(command).ConfigureAwait(false);

                    _logger.LogInformation("---- Started operation on bought products");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "---- Exception caught while product handling");
                }
            }
        }

        private static IEnumerable<PurchaseLineDto> MapToDto(IEnumerable<BoughtProduct> eventLines)
        {
            return eventLines.ToList().Select(MapToDto);
        }

        private static PurchaseLineDto MapToDto(BoughtProduct product)
        {
            return new PurchaseLineDto
            {
                Brand = product.Brand,
                Name = product.Name,
                Number = product.Number,
                OriginalPrice = product.OriginalPrice,
                Price = product.Price,
                Seller = Guid.Parse(product.Seller),
                CategoryId = product.CategoryId,
                UnitId = product.CategoryId
            };
        }
    }
}