param location string
param suffix string

resource appconfiguration 'Microsoft.AppConfiguration/configurationStores@2023-03-01' = {
  name: 'appcs-${suffix}'
  location: location
  sku: {
    name: 'standard'
  }
}

output appConfigurationName string = appconfiguration.name
