param(
  [string]$InputPath = "docs/presentation.de.md",
  [string]$OutputPath = "docs/presentation.de.html"
)

$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
Push-Location $repoRoot

try {
  node .\scripts\build-presentation.mjs --input $InputPath --output $OutputPath
  if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
  }
}
finally {
  Pop-Location
}
