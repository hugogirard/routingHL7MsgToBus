using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Consumer;

public class Transaction
{
    [JsonProperty("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty("senderId")]
    public string SenderId { get; set; }

    [JsonProperty("hl7Msg")]
    public string Hl7Msg { get; set; }

    [JsonProperty("msgProperties")]
    public Dictionary<string, object> MsgProperties { get; set; } = new();
}
