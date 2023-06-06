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
using System.Text;

namespace HL7Sender.Service;

public static class HL7MsgGenerator
{
    public static string GenerateAdt(string source, string destination,string adt_type)
    {
        string mshSegment = $"MSH|^~\\&|{source}|CONTOSO|{destination}|CONTOSO|202305311200||ADT^{adt_type}|123456789|P|2.5|||";
        string evnSegment = "EVN|A01|202305311200";
        string pidSegment = "PID|1||12345||Doe^John^^^Mr.|||123 Main St^^Los Angeles^CA^90001^US";

        // Add field 9.1 and 9.2 to the PID segment
        pidSegment += "|^ReferringDoctor^";

        // Combine the segments to form the complete message
        string adtMessage = $"{mshSegment}\r\n{evnSegment}\r\n{pidSegment}";

        return adtMessage;
    }    
}
