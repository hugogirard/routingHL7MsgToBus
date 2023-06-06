# Introduction

This GitHub repository show how to create routing in Azure Service Bus when receiving HL7 v2 messages.

This represent a HIS (Hospital Information System) sending HL7 v2 messages to an Azure Function.  In a real scenario, you will probably have a proxy between the HIS and the Azure Function.

Most of the HIS are still using MLLP (Minimal Lower Layer Protocol) to send HL7 v2 messages.  This protocol is not supported in Azure.  This is why we need to use a proxy.

The proxy will receive the HL7 v2 message from the HIS and send it to the Azure Function using HTTP.

In this scenario we will not simulate the proxy and just send message directly to an Azure Function.

Once the function receive the HL7 message it will read specific segments and fields from it.  Based on the [external configuration store pattern](https://learn.microsoft.com/en-us/azure/architecture/patterns/external-configuration-store)

# Architecture

The following diagram represent the architecture of this sample.  Keep in mind, we created a proxy to send the HL7 v2 message to the Azure Function.  In this sample we will not simulate the proxy and just send the message directly to the Azure Function.

![Architecture](./diagram/architecture.png)

1- The HIS send a HL7 v2 message to the Proxy (MLLP to HTTPS)
2- The Proxy send the HL7 v2 message to the Azure Function (HTTPS)
3- The Azure Function retrieve the routing configuration from Azure App Configuration
4- The Azure Function read the HL7 v2 message and extract the segments and fields needed to be added in the [message properties](https://learn.microsoft.com/en-us/rest/api/servicebus/message-headers-and-properties#message-properties).  Those properties will be used to route the message to the right topic subscription.  Once all user defined properties are added to the message, the Azure Function send the message to the Service Bus Topic.
5- In this scenario you have two consumers with their own subscription and filter.  Each receive the message that match their filter.
6 - Once the message retrieve it's saved to CosmosDB with the HL7 message and the message properties.

## Routing

The routing is based on configuration stored in Azure App Configuration.  The goal is to create extraction of specific segments and fields from the HL7 v2 message and add them as properties in the message without doing any code changes.

Here you can find the default configuration used in this sample in the directory **bicep/modules/appconfiguration/hl7.extraction.json**


# Prerequisites

- Fork this GitHub repository
- Create Service Principal needed for [GitHub Actions](https://github.com/marketplace/actions/azure-login#configure-a-service-principal-with-a-secret)
- Create GitHub Secrets

| Name | Value |
| ---- | ----- |
| AZURE_CREDENTIALS | Service Principal created above |
| AZURE_SUBSCRIPTION | Azure Subscription Id |
| PA_TOKEN | Personal Access Token ([PAT](https://github.com/marketplace/actions/create-github-secret-action#pa_token)) to access the GitHub repository |



# Create Azure Resources

Now, is time to create the Azure Resources, to do so just run the GitHub Actions called **Create Azure Resources**.

# Deploy ProcessHL7Msg Azure Function

Once the previous GitHub action executed succesfully