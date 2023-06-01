using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Consumer
{
    public partial class ConsumeHL7
    {
        private readonly ILogger _logger;

        public ConsumeHL7(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ConsumeHL7>();
        }

        [Function("ConsumeHL7")]        
        [CosmosDBOutput("%CosmosDb%", "%CosmosCollOut%", Connection = "CosmosConnection", CreateIfNotExists = true)]
        public object Run([ServiceBusTrigger("%ServiceBusTopicName%", "%SubscriptionName%", Connection = "ServiceBusCnxString")] string mySbMsg)
        {
            try
            {
                var document = new HL7Document
                {
                    Id = Guid.NewGuid().ToString(),
                    HL7Message = mySbMsg
                };

                return document;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }


        }
    }
}
