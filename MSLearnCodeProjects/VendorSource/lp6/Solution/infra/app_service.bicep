param libraryappweb_name string
param libraryappplan_name string
param location string

resource libraryappplan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: libraryappplan_name
  location: location
  sku: {
    name: 'B1'
    tier: 'Basic'
    size: 'B1'
    family: 'B'
    capacity: 1
  }
  properties: {
    reserved: true
  }
  kind: 'linux'
}

resource libraryappweb 'Microsoft.Web/sites@2023-01-01' = {
  name: libraryappweb_name
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
