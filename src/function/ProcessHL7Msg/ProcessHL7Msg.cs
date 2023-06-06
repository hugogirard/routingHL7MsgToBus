/*
* Notice: Any links, references, or attachments that contain sample scripts, code, or commands comes with the following notification.
*
* This Sample Code is provided for the purpose of illustration only and is not intended to be used in a production environment.
* THIS SAMPLE CODE AND ANY RELATED INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED,
* INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
*
* We grant You a nonexclusive, royalty-free right to use and modify the Sample Code and to reproduce and distribute the object code form of the Sample Code,
* provided that You agree:
*
* (i) to not use Our name, logo, or trademarks to market Your software product in which the Sample Code is embedded;
* (ii) to include a valid copyright notice on Your software product in which the Sample Code is embedded; and
* (iii) to indemnify, hold harmless, and defend Us and Our suppliers from and against any claims or lawsuits,
* including attorneys’ fees, that arise or result from the use or distribution of the Sample Code.
*
* Please note: None of the conditions outlined in the disclaimer above will superseded the terms and conditions contained within the Premier Customer Services Description.
*
* DEMO POC - "AS IS"
*/
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
