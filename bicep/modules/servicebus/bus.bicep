param location string
param suffix string


resource namespace 'Microsoft.ServiceBus/namespaces@2022-10-01-preview' = {
  name: 'bus-${suffix}'
  location: location
  sku: {
    name: 'Standard'
    tier: 'Standard'
  }
}

resource topic 'Microsoft.ServiceBus/namespaces/topics@2022-10-01-preview' = {
  name: 'integration'
  parent: namespace
  properties: {   
  }
}
