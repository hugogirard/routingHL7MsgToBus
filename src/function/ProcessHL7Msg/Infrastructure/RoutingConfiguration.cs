using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace ProcessHL7Msg.Infrastructure;

public class RoutingConfiguration
{    
    public string ArtifactName { get; set; }
    
    public List<HL7SegmentRouting> HL7SegmentRoutings { get; set; }

    public int Port { get; set; }
}

public class HL7SegmentRouting 
{
    public string SegmentName { get; set; }

    public int Position { get; set; }
}