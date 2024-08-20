param(
    [string]$dir,
    [string]$destinationDir
)

# Copy if doesn't exist
if (-not (Test-Path "$dir")) {
    Copy-Item -Path "$dir" -Destination "$destinationDir" -Recurse
}

