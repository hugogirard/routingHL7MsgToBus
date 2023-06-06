/*
* Notice: Any links, references, or attachments that contain sample scripts, code, or commands comes with the following notification.
*
* This Sample Code is provided for the purpose of illustration only and is not intended to be used in a production environment.
* THIS SAMPLE CODE AND ANY RELATED INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED,
* INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
*
* We grant You a nonexclusive, royalty-free right to use and modify the Sample Code and to reproduce and distribute the object code form of the Sample Code,
* provided that You agree:
*
* (i) to not use Our name, logo, or trademarks to market Your software product in which the Sample Code is embedded;
* (ii) to include a valid copyright notice on Your software product in which the Sample Code is embedded; and
* (iii) to indemnify, hold harmless, and defend Us and Our suppliers from and against any claims or lawsuits,
* including attorneys’ fees, that arise or result from the use or distribution of the Sample Code.
*
* Please note: None of the conditions outlined in the disclaimer above will superseded the terms and conditions contained within the Premier Customer Services Description.
*
* DEMO POC - "AS IS"
*/
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
        await bootstrapper.StartSendingAsync();        
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


