using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Contoso
{
    public class ProcessHL7Msg
    {
        private readonly ILogger _logger;
        private readonly IHL7Processor _hl7Processor;
        private readonly RoutingConfiguration _routingConfiguration;

        public ProcessHL7Msg(ILoggerFactory loggerFactory, 
                             IHL7Processor hL7Processor, 
                             RoutingConfiguration routingConfiguration)
        {
            _logger = loggerFactory.CreateLogger<ProcessHL7Msg>();
            _hl7Processor = hL7Processor;
            _routingConfiguration = routingConfiguration;            
        }

        [Function("ProcessHL7Msg")]                
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            string hl7Message = await new StreamReader(req.Body).ReadToEndAsync();            

            _hl7Processor.ProcessHL7Msg(hl7Message);

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions!");

            return response;
        }

        [Function("GetRoutingConfiguration")]
        public async Task<HttpResponseData> GetRoutingConfig([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            try
            {
                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

                var artifactName = _routingConfiguration.ArtifactName;
                var routing = _routingConfiguration.HL7SegmentRoutings;
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
