using System.Net;
using Azure.Messaging.ServiceBus;
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
            try
            {
                var subscriptionConfiguration = new SubscriptionConfiguration();
                _configuration.GetSection("Subscriptions").Bind(subscriptionConfiguration);

                string topicName = subscriptionConfiguration.TopicName;
                Azure.Response<SubscriptionProperties> serviceBusResponse;
                foreach (var subscription in subscriptionConfiguration.Subscriptions)
                {
                    bool createSubscription = false;
                    try
                    {
                        await _serviceBusClient.GetSubscriptionAsync(topicName, subscription.Name);
                    }
                    catch (ServiceBusException ex) when (ex.Reason == ServiceBusFailureReason.MessagingEntityNotFound)
                    {
                        createSubscription = true;
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                        continue;
                    }
                    
                    if (createSubscription) 
                    {
                        var subscriptionDescription = new CreateSubscriptionOptions(topicName, subscription.Name);
                        // Dot character are not supported in routing, so we replace it with underscore
                        var ruleOptions = new CreateRuleOptions("default",new SqlRuleFilter(subscription.Filter.Replace(".","_")));
                        serviceBusResponse = await _serviceBusClient.CreateSubscriptionAsync(subscriptionDescription,ruleOptions);
                        if (serviceBusResponse.GetRawResponse().IsError)
                        {
                            _logger.LogError("Cannot create subscription {subscriptionName} on topic {topicName}", subscription.Name, topicName);
                            continue;
                        }

                    }
                    
                }

                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

                response.WriteString("Welcome to Azure Functions!");

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting configuration");
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }


        }

        [Function("ManageSubscription")]
        public async Task<HttpResponseData> RecreateTopic([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req) 
        {
            try
            {
                var subscriptionConfiguration = new SubscriptionConfiguration();
                _configuration.GetSection("Subscriptions").Bind(subscriptionConfiguration);

                string topicName = subscriptionConfiguration.TopicName;

                try
                {
                    await _serviceBusClient.DeleteTopicAsync(topicName);
                }
                // Just in case the topics doesn't exist we don't want to throw an exception
                // just log the error
                catch (ServiceBusException ex) when (ex.Reason == ServiceBusFailureReason.MessagingEntityNotFound)
                {
                    _logger.LogError(ex,ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return req.CreateResponse(HttpStatusCode.InternalServerError);
                }

                var serviceBusResponse = await _serviceBusClient.CreateTopicAsync(topicName);

                if (serviceBusResponse.GetRawResponse().IsError) 
                { 
                    _logger.LogError("Cannot create topic {topicName}: StatusCode: {Status}", topicName,serviceBusResponse.GetRawResponse().Status);
                    return req.CreateResponse(HttpStatusCode.InternalServerError);
                }

                return req.CreateResponse(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting configuration");
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [Function("GetConfiguration")]
        public async Task<HttpResponseData> GetConfiguration([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            try
            {
                var subscriptionConfiguration = new SubscriptionConfiguration();
                _configuration.GetSection("Subscriptions").Bind(subscriptionConfiguration);

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(subscriptionConfiguration);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting configuration");
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }

        }
    }
}
