param libraryappapi_name string
param libraryappplan_name string
param location string

resource libraryappplan 'Microsoft.Web/serverfarms@2023-01-01' existing = {
  name: libraryappplan_name
}

resource libraryappweb 'Microsoft.Web/sites@2023-01-01' = {
  name: libraryappapi_name
  location: location
  kind: 'app,linux'
  properties: {
    enabled: true
    serverFarmId: libraryappplan.id
    siteConfig: {
      numberOfWorkers: 1
      linuxFxVersion: 'DOTNETCORE|8.0'
    }
    httpsOnly: true
    publicNetworkAccess: 'Enabled'
  }
}
