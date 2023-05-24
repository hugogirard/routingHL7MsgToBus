using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Contoso
{
    public class ProcessHL7Msg
    {
        private readonly ILogger _logger;

        public ProcessHL7Msg(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ProcessHL7Msg>();
        }

        [Function("ProcessHL7Msg")]                
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            string hl7Message = await new StreamReader(req.Body).ReadToEndAsync();
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions!");

            return response;
        }
    }
}
