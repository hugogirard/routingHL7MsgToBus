using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Consumer
{
    public class ConsumeHL7
    {
        private readonly ILogger _logger;

        public ConsumeHL7(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ConsumeHL7>();
        }

        [Function("ConsumeHL7")]
        public void Run([ServiceBusTrigger("%TopicName%", "%SubscriptionName%", Connection = "StrCnxString")] string mySbMsg)
        {
            _logger.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");
        }
    }
}
