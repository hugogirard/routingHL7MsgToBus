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
