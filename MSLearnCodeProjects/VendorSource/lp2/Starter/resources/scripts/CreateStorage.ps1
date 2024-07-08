param (
    [Parameter(Mandatory=$true, HelpMessage="Azure region to deploy to")]
    [string]$location,
    [Parameter(Mandatory=$true, HelpMessage="Target resource group name, will be created if doesn't exist")]
    [string]$resourceGroupName,
    [Parameter(Mandatory=$true, HelpMessage="Azure Storage Account name, will be created if doesn't exist")]
    [string]$storageAccountName
)

# Azure Storage Account if doesn't exist
$storageAccount = az storage account show --name $storageAccountName --resource-group $resourceGroupName --query id -o tsv
if (-not $storageAccount) {
    az deployment group create --resource-group $resourceGroupName --template-file ../../infra/storage.bicep --parameters storageAccount_name=$storageAccountName location=$location
}
