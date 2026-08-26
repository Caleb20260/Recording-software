$ErrorActionPreference = 'Stop'

$project = Join-Path $PSScriptRoot 'LubbInteractiveCreator.csproj'
$output = Join-Path $PSScriptRoot 'artifacts\publish\win-x64'
$package = Join-Path $PSScriptRoot 'artifacts\LubbInteractiveCreator-0.2.0-win-x64.zip'

dotnet publish $project -c Release -p:PublishProfile=win-x64
if (Test-Path $package) { Remove-Item $package -Force }
Compress-Archive -Path (Join-Path $output '*') -DestinationPath $package
Write-Host "Created $package"

$iscc = Get-Command ISCC.exe -ErrorAction SilentlyContinue
if ($iscc) {
	& $iscc.Source (Join-Path $PSScriptRoot 'installer\LubbInteractiveCreator.iss')
	Write-Host "Created installer in artifacts\installer"
} else {
	Write-Warning "Inno Setup was not found. Portable package created; install Inno Setup to build the setup executable."
}