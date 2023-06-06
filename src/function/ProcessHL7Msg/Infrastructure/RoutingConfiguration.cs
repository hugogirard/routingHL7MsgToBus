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

namespace ProcessHL7Msg.Infrastructure;

public class RoutingConfiguration
{
    public string ArtifactName { get; set; } = String.Empty;

    public List<HL7SegmentRouting> HL7SegmentRoutings { get; set; } = new List<HL7SegmentRouting>();

    public SesssionField SessionField { get; set; }

    public int Port { get; set; } = 0;
}

public class SesssionField 
{
    public string SegmentName { get; set; } = String.Empty;

    public int Position { get; set; } = 0;
}

public class HL7SegmentRouting 
{
    public string SegmentName { get; set; } = String.Empty;

    public int Position { get; set; } = 0;

    public int[] SubPosition { get; set; } = new int[] { };

    public string SubPositionSeparator { get; set; } = string.Empty;
}