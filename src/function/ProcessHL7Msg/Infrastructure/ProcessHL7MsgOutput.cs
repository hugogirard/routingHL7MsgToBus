using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessHL7Msg.Infrastructure;

public class ProcessHL7MsgOutput
{
    [ServiceBusOutput("integration", Connection = "ServiceBusConnection")]
    public ServiceBusMessage Message { get; set; }
    public HttpResponseData Response { get; set; }
}
