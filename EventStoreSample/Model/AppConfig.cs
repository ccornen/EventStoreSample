using System;
using System.Collections.Generic;
using System.Text;

namespace EventStoreSample.Model
{
    public class AppConfig
    {
        public EventStoreConfig EventStore { get; set; }

        public AppConfig()
        {
            EventStore = new EventStoreConfig();
        }
    }
    public class EventStoreConfig
    {
        public string Uri { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
}
