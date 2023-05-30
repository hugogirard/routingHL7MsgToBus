using System.Net;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SubscriptionAdmin.Model;

namespace SubscriptionAdmin
{
    public class SubsAdmin
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly ServiceBusAdministrationClient _serviceBusClient;

        public SubsAdmin(ILoggerFactory loggerFactory,
                        IConfiguration configuration,
                        ServiceBusAdministrationClient serviceBusAdministrationClient)
        {
            _logger = loggerFactory.CreateLogger<SubsAdmin>();
            _configuration = configuration;
            _serviceBusClient = serviceBusAdministrationClient;
        }

        [Function("ManageSubscription")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            var subscriptionConfiguration = new SubscriptionConfiguration();
            _configuration.GetSection("SubscriptionConfiguration").Bind(subscriptionConfiguration);

            string topicName = subscriptionConfiguration.TopicName;

            foreach (var subscription in subscriptionConfiguration.Subscriptions)
            {
                var serviceBusResponse = await _serviceBusClient.GetSubscriptionAsync(topicName, subscription.Name);

                if (serviceBusResponse == null || serviceBusResponse?.Value == null) 
                { 
                    // Create the subscription
                    await _serviceBusClient.CreateSubscriptionAsync(topicName, subscription.Name);
                }
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions!");

            return response;
        }
    }
}
