using EventStore.ClientAPI;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace EventStoreConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting");
            Start().Wait();
            Console.ReadKey();
        }

        static async Task Start()
        {
            var conn = EventStoreConnection.Create(new Uri("tcp://admin:changeit@localhost:1113"));
            await conn.ConnectAsync();
            await conn.ConnectToPersistentSubscriptionAsync("Activities", "ActivitiesGroup", OnNewEvent, OnSubscriptionDropped);
        }

        private static void OnSubscriptionDropped(EventStorePersistentSubscriptionBase arg1, SubscriptionDropReason arg2, Exception arg3)
        {
            Console.WriteLine("Subscription dropped");
        }

        private static Task OnNewEvent(EventStorePersistentSubscriptionBase arg1, ResolvedEvent arg2, int? arg3)
        {
            var json = System.Text.Encoding.UTF8.GetString(arg2.Event.Data);
            var activity = JsonSerializer.Deserialize<Activity>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });
            Console.WriteLine($"Activity received " + activity.Name + " " + activity.Other);
            return Task.CompletedTask;
        }
    }

    internal class Activity
    {
        public string Name { get; set; }
        public string Other { get; set; }
    }
}
