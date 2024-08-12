$ErrorActionPreference = "Stop"

. .\Common.ps1

H1 "LP3: Setting up Resources"
    
H2 "Copy Json Folder From Resources To Library.Console - Started"
& .\CopyFolder.ps1 -dir "..\Json" -destinationDir "..\..\src\Library.Console"
H2 "Copy Json Folder From Resources To Library.Console - Done"

H2 "Copy .vscode Folder From Resources - Started"
& .\CopyFolder.ps1 -dir ".\lp3\.vscode" -destinationDir "..\..\src\Library.Console"
H2 "Copy .vscode Folder From Resources - Done"

H1 "All Done"
