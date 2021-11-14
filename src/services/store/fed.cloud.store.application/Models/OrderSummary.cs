using System;

namespace fed.cloud.store.application.Models;

public record OrderSummary
{
    public string orderId { get; set; }

    public decimal total { get; set; }

    public DateTime date { get; set; }

    public int itemsTotal { get; set; }

    public string status { get; set; }

    internal static OrderSummary CreateIssuedOrder(string issueString)
    {
        return new OrderSummary
        {
            orderId = string.Empty,
            total = decimal.Zero,
            date = DateTime.Now,
            itemsTotal = 0,
            status = issueString
        };
    }
}