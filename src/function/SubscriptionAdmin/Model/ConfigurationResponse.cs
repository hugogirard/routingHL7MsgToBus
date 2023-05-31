using Microsoft.Azure.Functions.Worker.Http;

namespace SubscriptionAdmin.Model;

public class ConfigurationResponse
{
    public HttpResponseData ResponseData { get; set; }

    public SubscriptionConfiguration SubscriptionConfiguration { get; set; } = new();
}
