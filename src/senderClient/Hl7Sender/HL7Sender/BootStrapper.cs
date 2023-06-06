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
using HL7Sender.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace HL7Sender
{
    public class BootStrapper
    {
        private readonly string _sendApiUrl;
        private readonly string _recreateTopicApiUrl;
        private readonly string _createSubsApiUrl;
        private int _msgCount = 1;

        public BootStrapper(IConfiguration configuration)
        {
            _sendApiUrl = configuration["SendApiUrl"] ?? throw new ArgumentException("the functionUrl need to be present");
            _recreateTopicApiUrl = configuration["TopicApiUrl"] ?? throw new ArgumentException("the functionUrl need to be present");
            _createSubsApiUrl = configuration["SubsApiUrl"] ?? throw new ArgumentException("the functionUrl need to be present");
        }
        
        public async Task StartSendingAsync()
        {            
            var senders = new string[]{ "CONTOSO_SENDER_A","CONTOSO_SENDER_B" };
            var receivers = new string[] { "CONTOSO_RECEIVE_A", "CONTOSO_RECEIVE_B" };

            using (var httpClient = new HttpClient())
            {
                string msg = string.Empty;
                HttpResponseMessage response;
                for (int i = 0; i < _msgCount; i++)
                {
                    for (int y = 0; y < senders.Length; y++)
                    {
                        msg = HL7MsgGenerator.GenerateAdt(senders[y], receivers[y], "A01");
                        response = await httpClient.PostAsync(_sendApiUrl, new StringContent(msg));
                        if (!response.IsSuccessStatusCode) 
                        {
                            Console.WriteLine($"Error sending msg: {response.StatusCode}");
                        }
                        msg = HL7MsgGenerator.GenerateAdt(senders[y], receivers[y], "A02");
                        response = await httpClient.PostAsync(_sendApiUrl, new StringContent(msg));
                        if (!response.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"Error sending msg: {response.StatusCode}");
                        }
                        msg = HL7MsgGenerator.GenerateAdt(senders[y], receivers[y], "A03");
                        response = await httpClient.PostAsync(_sendApiUrl, new StringContent(msg));
                        if (!response.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"Error sending msg: {response.StatusCode}");
                        }
                        msg = HL7MsgGenerator.GenerateAdt(senders[y], receivers[y], "A04");
                        response = await httpClient.PostAsync(_sendApiUrl, new StringContent(msg));
                        if (!response.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"Error sending msg: {response.StatusCode}");
                        }
                        msg = HL7MsgGenerator.GenerateAdt(senders[y], receivers[y], "A05");
                        response = await httpClient.PostAsync(_sendApiUrl, new StringContent(msg));
                        if (!response.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"Error sending msg: {response.StatusCode}");
                        }
                        msg = HL7MsgGenerator.GenerateAdt(senders[y], receivers[y], "A06");
                        response = await httpClient.PostAsync(_sendApiUrl, new StringContent(msg));
                        if (!response.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"Error sending msg: {response.StatusCode}");
                        }
                    }

                }
            }
        }

        public async Task RecreateTopic() 
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(_recreateTopicApiUrl, null);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error creating topics msg: {response.StatusCode}");
                }
            }
        }

        public async Task CreateSubscription() 
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(_createSubsApiUrl, null);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error creating subscription: {response.StatusCode}");
                }
            }
        }
    }
}
