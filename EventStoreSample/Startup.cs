using EventStore.ClientAPI;
using EventStoreSample.Core;
using EventStoreSample.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace EventStoreSample
{
    public class Startup
    {
        AppConfig Config { get; }
        IConfiguration Configuration { get; }

        public ServiceProvider ServiceProvider { get; private set; }

        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
            Config = new AppConfig();
            Configuration.Bind(Config);
        }

        public async Task Run()
        {
            try
            {
                var storeManager = ServiceProvider.GetService<IEventStoreManager>();
                await storeManager.InitializeActivityStream();
                while (true)
                {
                    await Task.Delay(1000);
                    var activity = new Activity
                    {
                        Name = Guid.NewGuid().ToString(),
                        Other = Guid.NewGuid().ToString()
                    };
                    await storeManager.PublishActivity(activity, "someEventType");
                }
            }
            catch(Exception e)
            {

            }

        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Config);

            var conn = EventStoreConnection.Create(new Uri(Config.EventStore.Uri));
            conn.ConnectAsync().Wait();

            services.AddSingleton(conn);
            services.AddSingleton<IEventStoreManager, EventStoreManager>();

            //configure console logging
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(Configuration.GetSection("Logging"));
                loggingBuilder.AddConsole();
            });

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
