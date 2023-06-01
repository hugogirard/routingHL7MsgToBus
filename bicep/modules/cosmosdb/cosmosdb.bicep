param location string
param suffix string

var containerName = [
  'consumerA'
  'consumerB'
]

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

resource database 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2023-04-15' = {
  parent: account
  name: 'hl7'
  properties: {
    resource: {
      id: 'hl7'
    }
  }
}

resource container 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2023-04-15' = [for name in containerName: {
  parent: database
  name: name
  properties: {
    resource: {
      id: name
      partitionKey: {
        paths: [
          '/senderId'
        ]
        kind: 'Hash'
      }
    }
    options: {
      throughput: 400
    }
  }
}]

output cosmosDbName string = account.name
