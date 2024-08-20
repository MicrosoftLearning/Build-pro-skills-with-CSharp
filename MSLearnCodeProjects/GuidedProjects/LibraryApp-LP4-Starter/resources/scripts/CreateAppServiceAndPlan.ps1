param (
    [Parameter(Mandatory=$true, HelpMessage="Azure region to deploy to")]
    [string]$location,
    [Parameter(Mandatory=$true, HelpMessage="Target resource group name, will be created if doesn't exist")]
    [string]$resourceGroupName,
    [Parameter(Mandatory=$true, HelpMessage="Azure App Service name; globally unique; will be created if doesn't exist")]
    [string]$appServiceName
)

# Create App Service Plan if doesn't exist
$appServicePlan = az appservice plan show --name $appServiceName --resource-group $resourceGroupName --query id -o tsv
if (-not $appServicePlan) {
    az deployment group create --resource-group $resourceGroupName --template-file ../../infra/app_service.bicep --parameters libraryappplan_name="$($appServiceName)_plan" libraryappweb_name="$appServiceName" location=$location
}