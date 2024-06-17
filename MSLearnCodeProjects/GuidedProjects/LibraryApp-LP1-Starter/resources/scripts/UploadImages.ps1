param (
    [string] $storageAccountName
)

# Upload Images\patrons to "patrons" container
az storage blob upload-batch --account-name $storageAccountName --destination "patrons" --source "..\Images\patrons" --pattern "*.jpg" --overwrite false
# Upload Images\covers to "covers" container
az storage blob upload-batch --account-name $storageAccountName --destination "covers" --source "..\Images\covers" --pattern "*.jpg" --overwrite false