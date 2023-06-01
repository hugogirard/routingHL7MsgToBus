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

        client = new ServiceBusAdministrationClient(Environment.GetEnvironmentVariable("ServiceBusCnxString"));

        s.AddSingleton(client);
    })
    .Build();

host.Run();
