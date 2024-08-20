param(
    [Parameter(Mandatory=$true, HelpMessage="Azure region to deploy to")]
    [string]$location,
    [Parameter(Mandatory=$true, HelpMessage="Target resource group name, will be created if doesn't exist")]
    [string]$resourceGroupName,
    [Parameter(Mandatory=$true, HelpMessage="Azure SQL Server name, will be created if doesn't exist")]
    [string]$dbServerName
)

function GeneratePassword {
    param(
        [Parameter(Mandatory=$true, HelpMessage="Length of the password")]
        [int]$length
    )

    $chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_-+?|{}[]~"
    $password = ""
    for ($i = 0; $i -lt $length; $i++) {
        $char = $chars[(Get-Random -Minimum 0 -Maximum $chars.Length)]
        $password += $char
    }

    return $password
}


# Azure SQL Server if doesn't exist
$server = az sql server show --name $dbServerName --resource-group $resourceGroupName --query id -o tsv
if (-not $server) {
    # Generate random admin password  
    $adminPassword = GeneratePassword 12

    # Create Azure SQL Server and Database
    az deployment group create --resource-group $resourceGroupName --template-file ../../infra/sql_db.bicep --parameters appName=$dbServerName adminPassword="$adminPassword" location=$location
    # az sql server create --name $dbServerName --resource-group $resourceGroupName --location $location --admin-user "sqladmin" --admin-password $adminPassword

    # Get local IP address
    $localIp = (Invoke-RestMethod http://ipinfo.io/json).ip

    # Allow local IP to access the server
    az sql server firewall-rule create --resource-group $resourceGroupName --server $dbServerName --name "AllowLocalIp" --start-ip-address $localIp --end-ip-address $localIp
}
else {
    $adminPassword = "SQL Server already exists, use the existing password"
}
# Get the connection string
# $connectionString = az sql db show-connection-string --client ado.net --name $dbServerName --server $dbServerName --auth-type SqlPassword --admin-password $adminPassword --output tsv
$connectionString = az sql db show-connection-string --client ado.net --name $dbServerName --server $dbServerName --output tsv

@{
    connectionString = $connectionString
    adminLogin = "libraryadmin"
    adminPassword = $adminPassword
} 

