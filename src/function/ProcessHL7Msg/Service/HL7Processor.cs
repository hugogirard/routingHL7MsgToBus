using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace ProcessHL7Msg.Service;

public class HL7Processor : IHL7Processor
{    
    private readonly RoutingConfiguration _routingConfiguration;

    public RoutingConfiguration GetConfiguration() => _routingConfiguration;

    public HL7Processor(IConfiguration configuration)
    {
        _routingConfiguration = new RoutingConfiguration();
        configuration.GetSection("Routing").Bind(_routingConfiguration);     
    }

    public List<ServiceBusRoutingProperty> ProcessHL7Msg(string message)
    {
        var routingProperties = new List<ServiceBusRoutingProperty>();
         
        var segments = SplitSegments(message);

        foreach (var segment in segments)
        {
            var fields = SplitFields(segment);

            string segmentName = fields[0];

            var routingSegmentsConfig = _routingConfiguration.HL7SegmentRoutings.Where(s => s.SegmentName == segmentName);

            // Validate if the segment is part of the routing
            if (routingSegmentsConfig == null || routingSegmentsConfig.Count() == 0)
                continue;

            // Extract each field and position
            var positions = routingSegmentsConfig.Select(s => s).ToList();

            positions.ForEach(p => 
            {
                if (p.SubPosition?.Count() == 0 || string.IsNullOrEmpty(p.SubPositionSeparator))
                {                    
                    routingProperties.Add(new ServiceBusRoutingProperty
                    {
                        Name = $"{segmentName}.{p.Position}",
                        Value = fields[p.Position - 1]
                    });
                }
                else 
                {
                    string[] subValues = fields[p.Position - 1].Split(p.SubPositionSeparator);
                    foreach (var subPosition in p.SubPosition) 
                    {                            
                        routingProperties.Add(new ServiceBusRoutingProperty
                        {
                            Name = $"{segmentName}.{p.Position}.{subPosition}",
                            Value = subValues[subPosition - 1]
                        });
                    } 
                }


            });

        }

        return routingProperties;
    }

    private List<string> SplitSegments(string message) 
    {
        string[] separators = { "\r", "\n" };
        string[] segmentsArray = message.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        return new List<string>(segmentsArray);
    }

    private List<string> SplitFields(string segment) 
    {
        char[] separators = { '|' };
        string[] fieldsArray = segment.Split(separators);
        return new List<string>(fieldsArray);
    }
}


