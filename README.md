# Introduction

This GitHub repository show how to create routing in Azure Service Bus when receiving HL7 v2 messages.

This represent a HIS (Hospital Information System) sending HL7 v2 messages to an Azure Function.  In a real scenario, you will probably have a proxy between the HIS and the Azure Function.

Most of the HIS are still using MLLP (Minimal Lower Layer Protocol) to send HL7 v2 messages.  This protocol is not supported in Azure.  This is why we need to use a proxy.

The proxy will receive the HL7 v2 message from the HIS and send it to the Azure Function using HTTP.

In this scenario we will not simulate the proxy and just send message directly to an Azure Function.

Once the function receive the HL7 message it will read specific segments and fields from it.  Based on the [external configuration store pattern](https://learn.microsoft.com/en-us/azure/architecture/patterns/external-configuration-store)

## Function Admin

The admin function is using the external configuration store to create the subscription.  The subscription will be created in the Azure Service Bus topic.

From there you can configure your filter for each subscription in your system.

Before using the admin function you need configure some appsettings.  This is the key-value you need to configure:

| Name | Value |
| ---- | ----- |
| ServiceBusCnxString | The connection string of the Azure service bus with send rights |
| ServiceBusTopicName | integration |
| AppConfigurationCnxString | The configuration string of the app configuration |

## Function Routing

The routing function receive the HL7 v2 messages from the HIS system or by a proxy doing MLLP to HTTPS.  You need to configure some appsettings value.  This is the key-value you need to configure:

| Name | Value |
| ---- | ----- |
| ServiceBusCnxString | The connection string of the Azure service bus with send rights |
| ServiceBusTopicName | integration |
| AppConfigurationCnxString | The configuration string of the app configuration |