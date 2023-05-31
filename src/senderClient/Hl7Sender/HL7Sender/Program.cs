using HL7Sender;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json",
                                                            optional: true,
                                                            reloadOnChange: true)
                                              .AddUserSecrets<Program>()                                              
                                              .Build();

var serviceProvider = new ServiceCollection()
                          .AddSingleton(configuration)
                          .AddSingleton<IBootStrapper, BootStrapper>()
                          .BuildServiceProvider();


var bootstrapper = serviceProvider.GetService<IBootStrapper>();

if (bootstrapper.Init(args)) 
{
    await bootstrapper.StartSendingAsync();
}
