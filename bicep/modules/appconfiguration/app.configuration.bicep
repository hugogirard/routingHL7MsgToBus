param location string
param suffix string

resource appconfiguration 'Microsoft.AppConfiguration/configurationStores@2023-03-01' = {
  name: 'appconfiguration-${suffix}'
  location: location
  sku: {
    name: 'standard'
  }
}
