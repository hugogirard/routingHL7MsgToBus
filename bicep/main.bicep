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

module appConfiguration 'modules/appconfiguration/app.configuration.bicep' = {
  scope: resourceGroup(rg.name)
  name: 'appconfiguration'
  params: {
    location: location
    suffix: suffix
  }
}

module cosmosdb 'modules/cosmosdb/cosmosdb.bicep' = {
  scope: resourceGroup(rg.name)
  name: 'cosmosdb'
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
    appConfigurationName: appConfiguration.outputs.appConfigurationName
    srvNamespace: servicebus.outputs.serviceBusNamespaceName
    cosmosdbName: cosmosdb.outputs.cosmosDbName
  }
}

output appConfigurationStoreName string = appConfiguration.outputs.appConfigurationName
output functionProcessorName string = function.outputs.functionHL7ProcessorName
output functionAdminName string = function.outputs.functionHL7AdminName
output functionConsumerAName string = function.outputs.functionConsumerAName
output functionConsumerBName string = function.outputs.functionConsumerBName
