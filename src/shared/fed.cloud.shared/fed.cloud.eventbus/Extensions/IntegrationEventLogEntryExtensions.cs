using fed.cloud.eventbus.Base;
using fed.cloud.eventbus.Data;
using Newtonsoft.Json;
using System;

namespace fed.cloud.eventbus.Extensions
{
    public static class IntegrationEventLogEntryExtensions
    {
        public static IntegrationEventLogEntry Deserilize(this IntegrationEventLogEntry entry, Type toType)
        {
            entry.IntegrationEvent = (IntegrationEvent)JsonConvert.DeserializeObject(entry.Content, toType);
            return entry;
        }
    }
}
