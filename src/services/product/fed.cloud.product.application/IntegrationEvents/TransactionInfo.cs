using System;

namespace fed.cloud.product.application.IntegrationEvents;

public class TransactionInfo
{
    public TransactionInfo()
    {
        TransactionId = Guid.NewGuid();
    }

    public Guid TransactionId { get; }

    public int TotalEvents { get; private set; }

    public void UpdateEventsCount(int toChange)
    {
        TotalEvents = toChange;
    }

    public void UpdateEventsCount(Func<int, int> totalCount)
    {
        TotalEvents = totalCount(TotalEvents);
    }
}