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
    fileshare: 'fnccons98'
  }
  consumerB: {
    name: 'fnc-consumerB-${suffix}'
    cosmosCollectionName: 'consumerB'
    fileshare: 'fnccons99'
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
          value: account.listKeys().primaryMasterKey
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
          value: 'dotnet-isolated'
        }

      ]
      netFrameworkVersion: 'v7.0'
    }    
  }
}]

output functionHL7ProcessorName string = functionAppNames[0]
output functionHL7AdminName string = functionAppNames[1]
output functionConsumerAName string = 'fnc-consumerA-${suffix}'
output functionConsumerBName string = 'fnc-consumerB-${suffix}'
