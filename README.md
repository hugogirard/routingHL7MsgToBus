# Introduction

This GitHub repository show how to create routing in Azure Service Bus when receiving HL7 v2 messages.

This represent a HIS (Hospital Information System) sending HL7 v2 messages to an Azure Function.  In a real scenario, you will probably have a proxy between the HIS and the Azure Function.

Most of the HIS are still using MLLP (Minimal Lower Layer Protocol) to send HL7 v2 messages.  This protocol is not supported in Azure.  This is why we need to use a proxy.

The proxy will receive the HL7 v2 message from the HIS and send it to the Azure Function using HTTP.

In this scenario we will not simulate the proxy and just send message directly to an Azure Function.

Once the function receive the HL7 message it will read specific segments and fields from it.  Based on the [external configuration store pattern](https://learn.microsoft.com/en-us/azure/architecture/patterns/external-configuration-store)