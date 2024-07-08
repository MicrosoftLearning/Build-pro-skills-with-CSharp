param(
    [Parameter(Mandatory=$true, HelpMessage="Azure region to deploy to")]
    [string]$location,
    [Parameter(Mandatory=$true, HelpMessage="Target resource group name, will be created if doesn't exist")]
    [string]$resourceGroupName,
    [Parameter(Mandatory=$true, HelpMessage="Azure Storage Account name; globally unique; will be created if doesn't exist")]
    [string]$storageAccountName,
    [Parameter(Mandatory=$true, HelpMessage="Azure SQL Server name; globally unique; will be created if doesn't exist")]
    [string]$dbServerName,
    [Parameter(Mandatory=$true, HelpMessage="App Service name; globally unique; will be created if doesn't exist")]
    [string]$appServiceName
)

$ErrorActionPreference = "Stop"

. .\Common.ps1

H1 "LP5: Setting up Resources"
    
# Login to Azure
az login

H2 "Creating Resource Group - Started"
# Create resource group if doesn't exist
$resourceGroup = az group show --name $resourceGroupName --query id -o tsv
if (-not $resourceGroup) {
    az group create --name $resourceGroupName --location $location
}
H2 "Creating Resource Group - Done"

H2 "Azure Sql Server And Database Setup - Started"
$dbMeta = & .\CreateDb.ps1 -location $location -resourceGroupName $resourceGroupName -dbServerName $dbServerName
H2 "Azure Sql Server And Database Setup - Done"

H2 "Azure Storage Account Setup - Started"
& .\CreateStorage.ps1 -location $location -resourceGroupName $resourceGroupName -storageAccountName $storageAccountName
H2 "Azure Storage Account Setup - Done"

H2 "Azure App Service Setup - Started"
& .\CreateAppServiceAndPlan.ps1 -location $location -resourceGroupName $resourceGroupName -appServiceName $appServiceName
H2 "Azure App Service Setup - Done"

H2 "Uploading Files To Azure Storage - Started"
& .\UploadImages.ps1 -storageAccountName $storageAccountName
H2 "Uploading Files To Azure Storage - Done"

H2 "Copy Json Folder From Resources To Library.Web - Started"
& .\CopyFolder.ps1 -dir "..\Json" -destinationDir "..\..\src\Library.Web"
H2 "Copy Json Folder From Resources To Library.Web - Done"

H2 "Copy .vscode Folder From Resources - Started"
& .\CopyFolder.ps1 -dir ".\lp5\.vscode" -destinationDir "..\..\"
H2 "Copy .vscode Folder From Resources - Done"

H1 "Application Setup Instructions"

Write-Host "Storage Account URL: https://$storageAccountName.blob.core.windows.net"
Write-Host "Azure App Service URL: https://$appServiceName.azurewebsites.net"

Write-Host "Replace DefaultConnection in src\Library.Console\appSettings.json with the following connection string"
Write-Host """DefaultConnection"": ""$($dbMeta.connectionString)"""
Write-Host "username: $($dbMeta.adminLogin)"
Write-Host "password: $($dbMeta.adminPassword)"

Write-Host "After, run the following command in 'src\Library.Web' folder to create the database schema:"
Write-Host "dotnet ef database update"
