using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Contoso
{
    public class ProcessHL7Msg
    {
        private readonly ILogger _logger;
        private readonly IHL7Processor _hl7Processor;

        public ProcessHL7Msg(ILoggerFactory loggerFactory, IHL7Processor hL7Processor)
        {
            _logger = loggerFactory.CreateLogger<ProcessHL7Msg>();
            _hl7Processor = hL7Processor;
        }

        [Function("ProcessHL7Msg")]                
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            string hl7Message = await new StreamReader(req.Body).ReadToEndAsync();
            string test = Environment.GetEnvironmentVariable("FUNCTIONS_WORKER_RUNTIME", EnvironmentVariableTarget.Process);
            //_hl7Processor.ProcessHL7Msg(hl7Message);

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions!");

            return response;
        }
    }
}
