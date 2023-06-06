using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(builder => 
    {
        // This is a pre-release package do not use in production
        // register appinsignt like is done in ASP.NET Core
        builder.AddApplicationInsights()
               .AddApplicationInsightsLogger();
    })
    .ConfigureAppConfiguration(builder => 
    {
        string cnxString = Environment.GetEnvironmentVariable("AppConfigurationCnxString");
        builder.AddAzureAppConfiguration(options =>
        {
            options.Connect(cnxString)
                   .Select("Routing:*", "Contoso_HIS");
        });
    })
    .ConfigureServices(s => 
    {
        s.AddScoped<IHL7Processor, HL7Processor>();
        ServiceBusClient client;
        
        // In production you should use managed identity
        client = new ServiceBusClient(Environment.GetEnvironmentVariable("ServiceBusCnxString"));

        //// Topic name
        var sender = client.CreateSender(Environment.GetEnvironmentVariable("ServiceBusTopicName"));
        s.AddSingleton(sender);

       s.AddAzureAppConfiguration();
                          
    })
    //.ConfigureFunctionsWorkerDefaults(app => 
    //{
    //    // Use Azure App Configuration middleware for data refresh
    //    app.UseAzureAppConfiguration();
    //})
    .Build();

host.Run();
