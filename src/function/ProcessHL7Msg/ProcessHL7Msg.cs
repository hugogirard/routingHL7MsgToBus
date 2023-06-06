using System.Net;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Contoso
{
    public class ProcessHL7Msg
    {
        private readonly ILogger _logger;
        private readonly IHL7Processor _hl7Processor;
        private readonly ServiceBusSender _serviceBusSender;

        public ProcessHL7Msg(ILoggerFactory loggerFactory, 
                             IHL7Processor hL7Processor,
                             ServiceBusSender serviceBusSender)
        {
            _logger = loggerFactory.CreateLogger<ProcessHL7Msg>();
            _hl7Processor = hL7Processor;
            _serviceBusSender = serviceBusSender;
        }

        [Function("ProcessHL7Msg")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            try
            {
                _logger.LogInformation("C# HTTP trigger function processed a request.");
                string hl7Message = await new StreamReader(req.Body).ReadToEndAsync();
                
                var routingProperty = _hl7Processor.ProcessHL7Msg(hl7Message);

                var message = new ServiceBusMessage(hl7Message);
                foreach (var property in routingProperty)
                {
                    message.ApplicationProperties.Add(property.Name, property.Value);
                }
                // Set the sessionID, needed for FIFO
                message.SessionId = _hl7Processor.GetSessionIdValue(hl7Message);
                await _serviceBusSender.SendMessageAsync(message);
                var response = req.CreateResponse(HttpStatusCode.OK);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return req.CreateResponse(HttpStatusCode.InternalServerError);               
            }


        }

        [Function("GetHl7Configuration")]
        public async Task<HttpResponseData> GetRoutingConfig([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            try
            {
                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
     
                var routing = _hl7Processor.GetConfiguration();
                response.WriteString("Welcome to Azure Functions!");

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                var response = req.CreateResponse(HttpStatusCode.InternalServerError);
                response.WriteString("Internal server error");
                return response;
            }

        }
    }
}
