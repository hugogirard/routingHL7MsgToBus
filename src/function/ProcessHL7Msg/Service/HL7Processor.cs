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
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Reflection.Metadata.Ecma335;

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

    public string GetSessionIdValue(string message) 
    {
        var segments = SplitSegments(message);

        foreach (var segment in segments)
        {
            var fields = SplitFields(segment);

            string segmentName = fields[0];

            if (segmentName == _routingConfiguration.SessionField.SegmentName) 
            {
                return fields[_routingConfiguration.SessionField.Position - 1];
            }
        }

        return string.Empty;
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
                        // Dot character are not supported in routing, so we replace it with underscore
                        Name = $"{segmentName}_{p.Position}",
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
                            // Dot character are not supported in routing, so we replace it with underscore
                            Name = $"{segmentName}_{p.Position}_{subPosition}",
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


