namespace ProcessHL7Msg.Service;

public class HL7Processor : IHL7Processor
{
    private readonly RoutingConfiguration _routingConfiguration;

    public HL7Processor(RoutingConfiguration routingConfiguration)
    {
        _routingConfiguration = routingConfiguration;
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