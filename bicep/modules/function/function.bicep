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
param location string
param suffix string
param appInsightsName string
param appConfigurationName string
param srvNamespace string
param cosmosdbName string

resource account 'Microsoft.DocumentDB/databaseAccounts@2021-10-15' existing = {
  name: cosmosdbName
}

resource appInsights 'Microsoft.Insights/components@2020-02-02-preview' existing = {
  name: appInsightsName
}

resource appconfiguration 'Microsoft.AppConfiguration/configurationStores@2023-03-01' existing = {
  name: appConfigurationName
}

resource namespace 'Microsoft.ServiceBus/namespaces@2022-10-01-preview' existing = {
  name: srvNamespace 
}

// We return the default authorization rules from ServiceBus
resource defaultAuthorizationRules 'Microsoft.ServiceBus/namespaces/AuthorizationRules@2022-10-01-preview' existing = {
  name: 'RootManageSharedAccessKey'
  parent: namespace
}

resource storage 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: 'strf${suffix}'
  location: location
  sku: {
    name: 'Standard_LRS'    
  }
  kind: 'StorageV2'
  properties: {
    supportsHttpsTrafficOnly: true
    accessTier: 'Hot'
  }
}

resource serverFarm 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: 'asp-${suffix}'
  location: location
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
  kind: 'functionapp'
  properties: {    
  }
}

var functionAppNames = [
  'fnc-routing-${suffix}'
  'fnc-admin-${suffix}'
]

var consumerConfig = {
  consumerA: {
    name: 'fnc-consumerA-${suffix}'
    cosmosCollectionName: 'consumerA'
    subsname: 'ConsumerA'
    fileshare: 'fnccons98'
    topic: 'integration'
  }
  consumerB: {
    name: 'fnc-consumerB-${suffix}'
    cosmosCollectionName: 'consumerB'
    subsname: 'ConsumerB'
    fileshare: 'fnccons99'
    topic: 'integration'
  }  
}

resource functionHl7 'Microsoft.Web/sites@2022-09-01' = [for (name,i) in functionAppNames:{
  name: name
  location: location
  kind: 'functionapp'
  properties: {
    serverFarmId: serverFarm.id
    siteConfig: {
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.properties.InstrumentationKey
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsights.properties.ConnectionString
        }
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storage.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storage.listKeys().keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storage.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storage.listKeys().keys[0].value}'
        }
        {
          name: 'ServiceBusTopicName'
          value: 'integration'
        }
        {
          name: 'ServiceBusCnxString'
          value: defaultAuthorizationRules.listKeys().primaryConnectionString
        }
        {
          name: 'AppConfigurationCnxString'
          value: appconfiguration.listKeys().value[0].connectionString
        }
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: 'funcblobapp09${i}'
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }

      ]
      netFrameworkVersion: 'v7.0'
    }    
  }
}]

resource functionConsumer 'Microsoft.Web/sites@2022-09-01' = [for fnc in items(consumerConfig):{
  name: fnc.value.name
  location: location
  kind: 'functionapp'
  properties: {
    serverFarmId: serverFarm.id
    siteConfig: {
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.properties.InstrumentationKey
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsights.properties.ConnectionString
        }
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storage.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storage.listKeys().keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storage.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storage.listKeys().keys[0].value}'
        }
        {
          name: 'ServiceBusTopicName'
          value: 'integration'
        }
        {
          name: 'ServiceBusCnxString'
          value: defaultAuthorizationRules.listKeys().primaryConnectionString
        }
        {
          
          name: 'CosmosDb'
          value: 'AccountEndpoint=https://${cosmosdbName}.documents.azure.com:443/;AccountKey=${account.listKeys().primaryMasterKey};'
        }
        {
          name: 'Topic'
          value: fnc.value.topic
        }
        {
          name: 'Database'
          value: 'cosmosdb-${suffix}'
        }
        {
          name: 'SubsName'
          value: fnc.value.subsname
        }
        {
          name: 'CosmosCollOut'
          value: fnc.value.cosmosCollectionName
        }
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: fnc.value.fileshare
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet'
        }

      ]
      netFrameworkVersion: 'v6.0'
    }    
  }
}]

output functionHL7ProcessorName string = functionAppNames[0]
output functionHL7AdminName string = functionAppNames[1]
output functionConsumerAName string = 'fnc-consumerA-${suffix}'
output functionConsumerBName string = 'fnc-consumerB-${suffix}'
