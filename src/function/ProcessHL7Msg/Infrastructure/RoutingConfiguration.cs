using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace ProcessHL7Msg.Infrastructure;

public class RoutingConfiguration 
{
    //public string ArtifactName { get; set; }

    //public List<HL7SegmentRouting> HL7SegmentRoutings { get; set; }

    //public int Port { get; set; }


    private readonly IConfiguration _configuration;

    public RoutingConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;        
    }

    public string ArtifactName => _configuration["Routing:ARTIFACT_NAME"] ??
                                    throw new ArgumentException("ARTIFACT_NAME environment variable not set.");

    public List<HL7SegmentRouting> HL7SegmentRoutings => _configuration["Routing:HL7_SEGMENT_ROUTINGS"] != null ?
                                                            JsonConvert.DeserializeObject<List<HL7SegmentRouting>>(_configuration["Routing:HL7_SEGMENT_ROUTINGS"]) :
                                                            throw new ArgumentException("HL7_SEGMENT_ROUTINGS environment variable not set.");

    public string Port => _configuration["Routing:PORT"] ??
                            throw new ArgumentException("PORT environment variable not set.");
}

public class HL7SegmentRouting 
{
    public string SegmentName { get; set; }

    public int Position { get; set; }
}