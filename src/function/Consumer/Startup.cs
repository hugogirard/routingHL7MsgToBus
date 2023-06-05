using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

[assembly: FunctionsStartup(typeof(Consumer.Startup))]

namespace Consumer;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {        
        // In production you should use managed identity
        var client = new ServiceBusClient(Environment.GetEnvironmentVariable("ServiceBusCnxString"));
        builder.Services.AddSingleton(client);
    }
}