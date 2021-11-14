using System;

namespace fed.cloud.store.domain.Root.Order
{
    public class OrderStatus
    {
        public Guid Id { get; set; }

        public Guid Owner { get; set; }

        public int StatusId { get; set; }

        public string StatusName { get; set; }

        public bool IsCompleteState { get; set; }
    }
}