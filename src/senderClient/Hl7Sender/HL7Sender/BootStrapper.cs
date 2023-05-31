using HL7Sender.Service;
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
        private readonly string _functionUrl;
        private int _msgCount;

        public BootStrapper(IConfiguration configuration)
        {
            _functionUrl = configuration["functionUrl"] ?? throw new ArgumentException("the functionUrl need to be present");
        }

        public bool Init(string[] args)
        {
            if (args.Length != 1)
            {
                return false;
            }

            // Get the number of msg to send
            _msgCount = int.Parse(args[0]);

            return true;
        }
        public async Task StartSendingAsync()
        {
            using (var httpClient = new HttpClient())
            {
                string msg = string.Empty;
                for (int i = 0; i < _msgCount; i++)
                {
                    msg = HL7MsgGenerator.GenerateAdt("CONTOSO_HIS", "CONTOSO_LAB", "A01");                 
                    await httpClient.PostAsync(_functionUrl, new StringContent(msg));
                    msg = HL7MsgGenerator.GenerateAdt("CONTOSO_HIS", "CONTOSO_LAB", "A04");
                    await httpClient.PostAsync(_functionUrl, new StringContent(msg));
                }
            }
        }
    }
}
