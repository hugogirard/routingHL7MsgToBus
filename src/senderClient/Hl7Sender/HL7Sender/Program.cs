using HL7Sender;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json",
                                                            optional: true,
                                                            reloadOnChange: true)
                                              .AddUserSecrets<Program>()                                              
                                              .Build();

var bootstrapper = new BootStrapper(configuration);

if (bootstrapper.Init(new string[] { "1" })) 
{
    await bootstrapper.StartSendingAsync();
}
