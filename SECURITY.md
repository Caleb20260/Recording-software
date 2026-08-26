# Security and Privacy

- Recording, streaming, microphone, camera, and screen capture are explicit user
  actions and must remain visible in the application.
- The application runs as the signed-in user by default and never requests hidden
  administrator access.
- Stream keys and OAuth tokens belong in Windows Credential Manager or DPAPI-backed
  storage, never in logs or project files.
- Discord authentication must use official OAuth; passwords are never requested.
- Xbox functionality must use supported Microsoft/Xbox interfaces and approved
  capture-device workflows.
- Plugins and browser widgets require declared permissions and a restricted host.
- Diagnostics must sanitize credentials, tokens, private paths, and recording data.
- Recordings, clips, projects, and backups are never deleted automatically.