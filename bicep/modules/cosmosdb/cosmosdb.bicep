param location string
param suffix string


resource account 'Microsoft.DocumentDB/databaseAccounts@2021-10-15' = {
  name: 'cosmosdb-${suffix}'
  location: location
  kind: 'GlobalDocumentDB'
  properties: {
    locations: [
      {
        locationName: location
        failoverPriority: 0
      }
    ]
    databaseAccountOfferType: 'Standard'
    enableAutomaticFailover: false
  }
}

output cosmosDbName string = account.name
