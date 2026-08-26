$ErrorActionPreference = 'Stop'

$project = Join-Path $PSScriptRoot 'LubbInteractiveCreator.csproj'
$output = Join-Path $PSScriptRoot 'artifacts\publish\win-x64'
$projectXml = [xml](Get-Content -Path $project -Raw)
$version = $projectXml.Project.PropertyGroup.Version | Select-Object -First 1
if ([string]::IsNullOrWhiteSpace($version)) {
    throw "Could not determine the application version from $project."
}

$package = Join-Path $PSScriptRoot "artifacts\LubbInteractiveCreator-$version-win-x64.zip"

if (Test-Path $output) { Remove-Item $output -Recurse -Force }
New-Item -ItemType Directory -Path (Split-Path $package -Parent) -Force | Out-Null

dotnet publish $project -c Release -p:PublishProfile=win-x64
if (Test-Path $package) { Remove-Item $package -Force }
Compress-Archive -Path (Join-Path $output '*') -DestinationPath $package
Write-Host "Created $package"

$iscc = Get-Command ISCC.exe -ErrorAction SilentlyContinue
if ($iscc) {
    & $iscc.Source (Join-Path $PSScriptRoot 'installer\LubbInteractiveCreator.iss')
    if ($LASTEXITCODE -ne 0) {
        throw "Inno Setup failed with exit code $LASTEXITCODE."
    }
    Write-Host "Created installer in artifacts\installer"
} else {
    Write-Warning "Inno Setup was not found. Portable package created; install Inno Setup to build the setup executable."
}