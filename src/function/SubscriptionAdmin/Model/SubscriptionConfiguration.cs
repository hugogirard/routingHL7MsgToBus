using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionAdmin.Model
{
    public class SubscriptionConfiguration
    {
        public string TopicName { get; set; } = string.Empty;

        public List<Subscription> Subscriptions { get; set; } = new();
    }

    public class Subscription
    {
        public string Name { get; set; } = string.Empty;

        public string Filter { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        // We this option you need to be carefull, all message in the current subscription will
        // be deleted and the subscription will be recreated
        public bool Recreate { get; set; }
    }
}
