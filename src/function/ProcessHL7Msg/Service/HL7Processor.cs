using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace ProcessHL7Msg.Service;

public class HL7Processor : IHL7Processor
{    
    private readonly RoutingConfiguration _routingConfiguration;

    public RoutingConfiguration GetConfiguration() => _routingConfiguration;

    public HL7Processor(IConfiguration configuration)
    {
        string conf = "{\"ARTIFACT_NAME\":\"RP_MLLP_VPPCerner_from_External_HL7_ALL\",\"HL7_SEGMENT_ROUTINGS\":[{\"SegmentName\":\"MSG\",\"Position\":3},{\"SegmentName\":\"MSG\",\"Position\":4},{\"SegmentName\":\"MSG\",\"Position\":5},{\"SegmentName\":\"MSG\",\"Position\":6}],\"Port\":12001}";
        _routingConfiguration = JsonConvert.DeserializeObject<RoutingConfiguration>(conf);        
    }

    public List<ServiceBusRoutingProperty> ProcessHL7Msg(string message)
    {
        var routingProperties = new List<ServiceBusRoutingProperty>();
         
        var segments = message.Split('|');

        foreach (var segment in segments)
        {
            string[] fields = segment.Split('|');

            string segmentName = fields[0];

            var routingSegmentsConfig = _routingConfiguration.HL7SegmentRoutings.Where(s => s.SegmentName == segmentName);

            // Validate if the segment is part of the routing
            if (routingSegmentsConfig == null || routingSegmentsConfig.Count() == 0)
                continue;

            // Extract each field and position
            var positions = routingSegmentsConfig.Select(s => s.Position).ToList();

            positions.ForEach(p => 
            {
                var routingProperty = new ServiceBusRoutingProperty
                {
                    Name = $"{segmentName}_{p}",
                    Value = fields[p]
                };

                routingProperties.Add(routingProperty);
            });

        }

        return routingProperties;
    }
}