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

        public ConsumeHL7(ILogger<ConsumeHL7> log)
        {
            _logger = log;
        }

        [FunctionName("ConsumeHl7")]
        public async Task Run([ServiceBusTrigger("integration", "%SubsName%", Connection = "ServiceBusCnxString")]ServiceBusReceivedMessage mySbMsg,
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
                
                foreach (var prop in mySbMsg.ApplicationProperties)
                {
                    transaction.MsgProperties.Add(prop.Key, prop.Value);
                }
                
                await transactions.AddAsync(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                // To be sure the msg is back in the queue since it runs in peeklock
                // https://learn.microsoft.com/en-us/azure/azure-functions/functions-bindings-service-bus-trigger?tabs=python-v2%2Cin-process%2Cextensionv5&pivots=programming-language-csharp#usage
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
