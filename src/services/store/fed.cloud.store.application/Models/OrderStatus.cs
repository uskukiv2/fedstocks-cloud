using System;

namespace fed.cloud.store.application.Models;

public record OrderStatusData
{
    public Guid Owner { get; set; }

    public int Id { get; set; }

    public string Name { get; set; }

    public bool IsCompleteState { get; set; }
}