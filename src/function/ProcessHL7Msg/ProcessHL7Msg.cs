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

        public ProcessHL7Msg(ILoggerFactory loggerFactory, 
                             IHL7Processor hL7Processor,
                             ServiceBusSender serviceBusSender)
        {
            _logger = loggerFactory.CreateLogger<ProcessHL7Msg>();
            _hl7Processor = hL7Processor;
        }

        [Function("ProcessHL7Msg")]
        public async Task<ProcessHL7MsgOutput> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            try
            {
                _logger.LogInformation("C# HTTP trigger function processed a request.");
                string hl7Message = await new StreamReader(req.Body).ReadToEndAsync();

                ProcessHL7MsgOutput processHL7MsgOutput = new();

                var routingProperty = _hl7Processor.ProcessHL7Msg(hl7Message);

                var message = new ServiceBusMessage(hl7Message);
                foreach (var property in routingProperty)
                {
                    message.ApplicationProperties.Add(property.Name, property.Value);
                }

                var response = req.CreateResponse(HttpStatusCode.OK);
                processHL7MsgOutput.Message = message;
                processHL7MsgOutput.Response = response;

                return processHL7MsgOutput;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new ProcessHL7MsgOutput
                {
                    Response = req.CreateResponse(HttpStatusCode.InternalServerError)
                };
            }


        }

        [Function("GetRoutingConfiguration")]
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
