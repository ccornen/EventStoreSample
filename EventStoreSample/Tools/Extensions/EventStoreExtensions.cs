using EventStore.ClientAPI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace EventStoreSample.Tools.Extensions
{
    public static class EventStoreExtensions
    {
        public static EventData ToEventData<T>(this T obj, string eventType, object metadata = null) where T : class
        {
            var data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(obj, 
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                }
                ));;
            var metadataBin = metadata == null
                ? Encoding.UTF8.GetBytes("{}")
                : Encoding.UTF8.GetBytes(JsonSerializer.Serialize(metadata));
            var eventPayload = new EventData(Guid.NewGuid(), eventType, true, data, metadataBin);
            return eventPayload;
        }
    }
}
