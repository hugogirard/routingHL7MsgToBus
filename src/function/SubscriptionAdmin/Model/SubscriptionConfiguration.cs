using System;
using System.Collections.Generic;
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
        public string Name { get; set; }

        public string Filter { get; set; }
    }
}
