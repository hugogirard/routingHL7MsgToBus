using HL7Sender;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json",
                                                            optional: true,
                                                            reloadOnChange: true)
                                              .AddUserSecrets<Program>()                                              
                                              .Build();

var bootstrapper = new BootStrapper(configuration);

Console.WriteLine("Enter the command you want to execute");
Console.WriteLine("1 - Send HL7 messages");
Console.WriteLine("2 - Recreate the topic");
Console.WriteLine("3 - Create the subscriptions");
var key = Console.ReadKey();

switch (key.KeyChar)
{
    case '1':
        Console.WriteLine("Enter the number of messages to send");
        var msgCount = Console.ReadLine();
        if (bootstrapper.Init(new string[] { msgCount }))
        {
            await bootstrapper.StartSendingAsync();
        }
        break;
    case '2':
        await bootstrapper.RecreateTopic();
        break;
    case '3':
        await bootstrapper.CreateSubscription();
        break;
    default:
        Console.WriteLine("Invalid selection");
        break;
}

Console.WriteLine("Press any key to exit");
Console.ReadKey();


