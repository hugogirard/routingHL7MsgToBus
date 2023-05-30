using Azure.Identity;
using Azure.Messaging.ServiceBus;
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
                   .Select("Routing:*", "Contoso_HIS");
        });
    })
    .ConfigureServices(s => 
    {
        s.AddScoped<IHL7Processor, HL7Processor>();
        
        // Create service bus client
        var client = new ServiceBusClient(Environment.GetEnvironmentVariable("ServiceBusFQDN"),
                                          new DefaultAzureCredential());

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
