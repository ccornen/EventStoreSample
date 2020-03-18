using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using EventStoreSample.Model;
using EventStoreSample.Tools.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace EventStoreSample.Core
{
    public class EventStoreManager : IEventStoreManager
    {
        private readonly IEventStoreConnection conn;
        private readonly AppConfig config;
        private readonly UserCredentials creds;

        public EventStoreManager(IEventStoreConnection conn, AppConfig config, ILogger<IEventStoreManager> logger)
        {
            this.conn = conn;
            this.config = config;
            this.Logger = logger;
            this.creds = new UserCredentials(config.EventStore.User, config.EventStore.Password);
        }

        public ILogger<IEventStoreManager> Logger { get; }

        public async Task DeleteActivitliesStream()
        {
            await conn.DeletePersistentSubscriptionAsync(Constants.StreamNameActivities, Constants.GroupNameActivities, creds);
        }

        public async Task InitializeActivityStream()
        {
            Logger.LogInformation("Initialize");

            await DeleteActivitliesStream();

            var conf =
                           PersistentSubscriptionSettings
                               .Create()
                               .StartFromBeginning()
                               .ResolveLinkTos()
                               .CheckPointAfter(TimeSpan.FromSeconds(1))
                               .MinimumCheckPointCountOf(1)
                               .MaximumCheckPointCountOf(10)
                               .Build();

            await conn.CreatePersistentSubscriptionAsync(Constants.StreamNameActivities,
                Constants.GroupNameActivities, conf,
                creds);
        }

        public async Task PublishActivity(Activity activity, string eventType)
        {
            Logger.LogInformation("Publishing activity");
            var creds = new UserCredentials(config.EventStore.User, config.EventStore.Password);
            await conn.AppendToStreamAsync(Constants.StreamNameActivities, ExpectedVersion.Any, creds, new[] { activity.ToEventData(eventType) });
        }
    }

    public interface IEventStoreManager
    {
        Task InitializeActivityStream();
        Task DeleteActivitliesStream();
        Task PublishActivity(Activity activity, string eventType);
    }
}
