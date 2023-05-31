using System.Text;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace HL7Sender.Service;

public static class HL7MsgGenerator
{
    public static string GenerateAdt(string source, string destination,string adt_type)
    {
        string mshSegment = $"MSH|^~\\&|CONTOSO_HIS|CONTOSO|CONTOSO_LAB|CONTOSO_CA|202305311200||ADT^{adt_type}|123456789|P|2.5|||";
        string evnSegment = "EVN|A01|202305311200";
        string pidSegment = "PID|1||12345||Doe^John^^^Mr.|||123 Main St^^Los Angeles^CA^90001^US";

        // Add field 9.1 and 9.2 to the PID segment
        pidSegment += "|^ReferringDoctor^";

        // Combine the segments to form the complete message
        string adtMessage = $"{mshSegment}\r\n{evnSegment}\r\n{pidSegment}";

        return adtMessage;
    }    
}
