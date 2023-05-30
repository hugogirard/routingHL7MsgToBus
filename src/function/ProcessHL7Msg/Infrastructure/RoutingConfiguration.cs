using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace ProcessHL7Msg.Infrastructure;

public class RoutingConfiguration
{
    public string ArtifactName { get; set; } = String.Empty;

    public List<HL7SegmentRouting> HL7SegmentRoutings { get; set; } = new List<HL7SegmentRouting>();

    public int Port { get; set; } = 0;
}

public class HL7SegmentRouting 
{
    public string SegmentName { get; set; } = String.Empty;

    public int Position { get; set; } = 0;
}