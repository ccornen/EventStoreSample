using Microsoft.Extensions.DependencyInjection;
using System;

namespace EventStoreSample
{
    class Program
    {
        static void Main()
        {
            var startup = new Startup();
            startup.ConfigureServices(new ServiceCollection());
            startup.Run().Wait();
            Console.ReadKey();
        }
    }
}
