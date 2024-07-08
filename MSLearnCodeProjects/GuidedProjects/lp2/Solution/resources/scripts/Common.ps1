function Write-Header($header) {
  $previousColor = $host.UI.RawUI.ForegroundColor
  $host.UI.RawUI.ForegroundColor = "Green"

  Write-Host ""
  Write-Host $header
  Write-Host ""

  $host.UI.RawUI.ForegroundColor = $previousColor
}

function H1 {
  param (
    [string] $header
  )
  Write-Header $header.ToUpper()
}


function H2 {
  param (
    [string] $header
  )
  Write-Header $header  
}