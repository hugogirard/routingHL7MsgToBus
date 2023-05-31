targetScope='subscription'

param location string

var rgName = 'rg-srvbus-routing'

var suffix = uniqueString(rg.id)

resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: rgName
  location: location
}

module servicebus 'modules/servicebus/bus.bicep' = {
  scope: resourceGroup(rg.name)
  name: 'servicebus'
  params: {
    location: location
    suffix: suffix
  }
}

module monitoring 'modules/monitoring/insights.bicep' = {
  scope: resourceGroup(rg.name)
  name: 'monitoring'
  params: {
    location: location
    suffix: suffix
  }
}

module function 'modules/function/function.bicep' = {
  scope: resourceGroup(rg.name)
  name: 'function'
  params: {
    appInsightsName: monitoring.outputs.appInsightName
    location: location
    suffix: suffix
  }
}

module appConfiguration 'modules/appconfiguration/app.configuration.bicep' = {
  scope: resourceGroup(rg.name)
  name: 'appconfiguration'
  params: {
    location: location
    suffix: suffix
  }
}

output appConfigurationStoreName string = appConfiguration.outputs.appConfigurationName
output functionProcessorName string = function.outputs.functionHL7ProcessorName
output functionAdminName string = function.outputs.functionHL7AdminName
output functionConsumerAName string = function.outputs.functionConsumerAName
output functionConsumerBName string = function.outputs.functionConsumerBName
