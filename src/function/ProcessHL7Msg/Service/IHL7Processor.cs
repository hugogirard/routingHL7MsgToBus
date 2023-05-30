namespace ProcessHL7Msg.Service;

public interface IHL7Processor
{
    List<ServiceBusRoutingProperty> ProcessHL7Msg(string message);

    public RoutingConfiguration GetConfiguration();
}
