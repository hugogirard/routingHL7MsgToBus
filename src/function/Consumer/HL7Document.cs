namespace Consumer;

public class HL7Document 
{
    public string Id { get; set; }

    public string HL7Message { get; set; }

    public DateTime TimeProcessed { get; set; } = DateTime.UtcNow;

    public int MyProperty { get; set; }
}

