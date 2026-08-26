#!/usr/bin/env bash
set -euo pipefail

root="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
version="0.2.0"
output="$root/artifacts/publish/win-x64"
package="$root/artifacts/LubbInteractiveCreator-${version}-win-x64.zip"

dotnet publish "$root/LubbInteractiveCreator.csproj" -c Release -p:PublishProfile=win-x64
rm -f "$package"
mkdir -p "$(dirname "$package")"
(cd "$output" && zip -qr "$package" .)
printf 'Created %s\n' "$package"

if command -v ISCC.exe >/dev/null 2>&1; then
	ISCC.exe "$root/installer/LubbInteractiveCreator.iss"
	printf 'Created installer in %s\n' "$root/artifacts/installer"
else
	printf 'Inno Setup was not found; portable package created.\n'
fi