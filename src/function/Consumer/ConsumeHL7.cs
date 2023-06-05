using System;
using System.Configuration;
using System.Net.Security;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Consumer
{
    public class ConsumeHL7
    {
        private readonly ILogger<ConsumeHL7> _logger;
        private readonly ServiceBusClient _serviceBusClient;
        private readonly string _topic;
        private readonly string _subscriptionName;

        public ConsumeHL7(ILogger<ConsumeHL7> log, ServiceBusClient serviceBusClient)
        {
            _logger = log;
            _serviceBusClient = serviceBusClient;
            _topic = "integration";
            _subscriptionName = Environment.GetEnvironmentVariable("SubsName");
        }

        [FunctionName("ConsumeHl7")]
        public async Task Run([ServiceBusTrigger("integration", "%SubsName%", Connection = "ServiceBusCnxString",AutoCompleteMessages = false)]ServiceBusReceivedMessage mySbMsg,
                              [CosmosDB("hl7", "%CosmosCollOut%", Connection = "CosmosDb")]IAsyncCollector<Transaction> transactions)           
        {
            try
            {
                string senderId = ExtractSenderId(mySbMsg.Body.ToString());
                                        

                Transaction transaction = new() 
                { 
                    Hl7Msg = mySbMsg.Body.ToString(),
                    SenderId = senderId
                };
                var receiver = _serviceBusClient.CreateReceiver(_topic, _subscriptionName);
                
                foreach (var prop in mySbMsg.ApplicationProperties)
                {
                    transaction.MsgProperties.Add(prop.Key, prop.Value);
                }

                //await receiver.CompleteMessageAsync(mySbMsg);
                string jsonObject = JsonConvert.SerializeObject(transaction);

                await transactions.AddAsync(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);         
                // To be sure the msg is back in the queue
                throw new Exception("Internal server error");
            }
        }

        private string ExtractSenderId(string hl7Message) 
        {
            // Extracting the MSH segment from the HL7 message
            string[] segments = hl7Message.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            string mshSegment = segments[0];

            // Splitting the MSH segment into fields
            string[] fields = mshSegment.Split('|');

            // Extracting the sender ID from the fields
            if (fields.Length > 2)
            {
                string senderComponent = fields[2];

                return senderComponent;
            }

            return string.Empty;
        }
    }
}
