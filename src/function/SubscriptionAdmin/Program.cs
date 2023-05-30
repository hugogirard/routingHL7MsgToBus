using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(builder => 
    {
        string cnxString = Environment.GetEnvironmentVariable("AppConfigurationCnxString");
        builder.AddAzureAppConfiguration(options =>
        {
            options.Connect(cnxString)
                   .Select("Subscriptions:*", "dev");
        });
    })
    .ConfigureServices(s => 
    {
        ServiceBusAdministrationClient client;
#if DEBUG
        client = new ServiceBusAdministrationClient(Environment.GetEnvironmentVariable("ServiceBusCnxString"));
#else
        // Create service bus client
        var client = new ServiceBusClient(Environment.GetEnvironmentVariable("ServiceBusFQDN"),
                                          new DefaultAzureCredential());
#endif

        s.AddSingleton(client);
    })
    .Build();

host.Run();