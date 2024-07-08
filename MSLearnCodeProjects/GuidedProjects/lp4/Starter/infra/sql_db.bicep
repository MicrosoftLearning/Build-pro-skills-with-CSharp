param appName string = 'libraryapp'
@secure()
param adminPassword string
param location string

resource libraryapp_server 'Microsoft.Sql/servers@2023-05-01-preview' = {
  name: appName
  location: location
  properties: {
    administratorLogin: 'libraryadmin'
    administratorLoginPassword: adminPassword
    version: '12.0'
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
    restrictOutboundNetworkAccess: 'Disabled'
  }
}

// a small SQL database with only basic features, 5 DTUs, and 1 GB of storage
resource libraryapp_database 'Microsoft.Sql/servers/databases@2023-05-01-preview' = {
  parent: libraryapp_server
  name: appName
  location: location
  sku: {
    name: 'Basic'
    tier: 'Basic'
    capacity: 5
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 1073741824
    catalogCollation: 'SQL_Latin1_General_CP1_CI_AS'
    zoneRedundant: false
    readScale: 'Disabled'
    requestedBackupStorageRedundancy: 'Local'
    isLedgerOn: false
    availabilityZone: 'NoPreference'
  }
}
