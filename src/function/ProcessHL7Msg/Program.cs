using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(s => 
    {
        s.AddScoped<IHL7Processor, HL7Processor>();
        s.AddSingleton<RoutingConfiguration>();
        
        // Create service bus client
        var client = new ServiceBusClient(Environment.GetEnvironmentVariable("ServiceBusFQDN"),
                                          new DefaultAzureCredential());

        //// Topic name
        var sender = client.CreateSender(Environment.GetEnvironmentVariable("ServiceBusTopicName"));
        s.AddSingleton<ServiceBusSender>(sender);
    })
    .Build();

host.Run();
