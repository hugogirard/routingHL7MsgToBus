namespace ProcessHL7Msg.Service;

public interface IHL7Processor
{
    List<ServiceBusRoutingProperty> ProcessHL7Msg(string message);

    string GetSessionIdValue(string message);

    public RoutingConfiguration GetConfiguration();
}
