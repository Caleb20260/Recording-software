# Distribution Packaging

## Microsoft Store / MSIX

`Package.appxmanifest` is the package identity and capability declaration for a
Windows Store submission. Before building an MSIX, replace the placeholder
`Identity` values with the exact Name and Publisher values assigned by Partner
Center, add signed PNG artwork under `packaging/Assets`, and sign the package with
the certificate associated with that identity.

The package requests only internet, microphone, and webcam capabilities. Screen
capture still requires the official Windows capture consent flow at runtime.

The Windows SDK tools `makeappx.exe` and `signtool.exe` are required to create and
sign the final MSIX. Microsoft Store submission additionally requires a Partner
Center account, Store listing assets, privacy policy, certification, and an approved
publisher identity. No Store URL is claimed until Microsoft assigns one.

## Other channels

- `package-release.ps1` creates the portable ZIP and classic Inno Setup installer.
- `package-release.sh` creates the portable ZIP in Linux/CI environments.
- `winget/LubbInteractive.LubbInteractiveCreator.yaml` is a submission template; set
  the final HTTPS installer URL and SHA-256 after publishing a signed installer.