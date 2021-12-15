using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace fed.cloud.eventbus.Base
{
    public record IntegrationEvent
    {
        public IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        [JsonConstructor]
        public IntegrationEvent(Guid id, DateTime creationDate)
        {
            Id = id;
            CreationDate = creationDate;
        }

        [JsonProperty]
        public Guid Id { get; }

        [JsonProperty]
        public DateTime CreationDate { get; }
    }
}
