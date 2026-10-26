# Open in CodeBuddy

A Visual Studio extension that adds a menu command letting you open any solution, project, folder or file in **CodeBuddy** (including CodeBuddy CN).

It is a port of [OpenInVsCode](https://github.com/madskristensen/OpenInVsCode) by Mads Kristensen, retargeted to CodeBuddy.

## Features

- Right-click any solution, project, folder or file in **Solution Explorer** → **Open in CodeBuddy**
  - Solutions open their folder, projects their root folder, files and folders open themselves
  - Multiple selected files are passed as multiple file arguments
- Top menu **Open in CodeBuddy** for the currently active document, carrying the cursor position
  - Default keyboard shortcut: `Ctrl+Shift+Y`
- If CodeBuddy is not found in its default location, it is located automatically via
  1. the `CodeBuddy CN` shell verb registered by the installer (`HKCU\...\Classes\*\shell\CodeBuddy CN`),
  2. the `buddycn` CLI shim on `PATH`,
  3. well-known install folders on any fixed drive,
  and as a last resort a file dialog asks you to point at `CodeBuddy CN.exe`.

## Options

Under **Tools → Options → Web → Open in CodeBuddy**:

| Option | Description |
|---|---|
| Path to CodeBuddy executable | Full path to `CodeBuddy CN.exe` |
| Command line arguments | Extra arguments appended to every launch |
| Open solution/project as regular file | Open the `.sln`/`.csproj` file itself instead of its folder |

## How it launches CodeBuddy

The extension prefers the CodeBuddy CLI (`resources\app\out\cli.js` next to the executable), started in
Electron's node mode (`ELECTRON_RUN_AS_NODE=1`) — exactly what the bundled `bin\buddycn.cmd` does, minus
the console window. This gives full `code`-compatible argument handling, including `-g file:line:column`
for jumping to the cursor position. If `cli.js` is not found, the GUI executable is started directly.

## Requirements

- Visual Studio 2017, 2019, 2022 (amd64/arm64) or 2026
- CodeBuddy installed locally

## Building

```cmd
msbuild OpenInCodeBuddy.sln /t:Restore;Build /p:Configuration=Release /p:DeployExtension=false
```

The resulting VSIX lands in `src/bin/Release/OpenInCodeBuddy.vsix` — double-click to install, or press `F5`
in Visual Studio to debug in the experimental instance.

## Publishing to the Marketplace

1. The publisher is already configured as **`xibeiwind`** in `src/source.extension.vsixmanifest`
   (`Identity Publisher`) and in [`publish/vs-publish.json`](publish/vs-publish.json). If you fork this and
   publish under another publisher, change both.
2. Create an Azure DevOps Personal Access Token with organization *All accessible organizations* and scope
   **Marketplace → Manage**.
3. Bump the version in three places (`vsixmanifest`, `source.extension.cs`, `AssemblyInfo.cs`), rebuild, then run:

```cmd
"C:\Program Files\Microsoft Visual Studio\2022\Enterprise\VSSDK\VisualStudioIntegration\Tools\Bin\VsixPublisher.exe" publish ^
  -payload "src\bin\Release\OpenInCodeBuddy.vsix" ^
  -publishManifest "publish\vs-publish.json" ^
  -personalAccessToken "%MARKETPLACE_PAT%"
```

4. Confirm the listing on the marketplace management page — a command-line upload is not necessarily public
   until you publish it there.

## License

[MIT](LICENSE)

The extension icon is derived from the CodeBuddy logo shipped with the CodeBuddy installation
(`resources\app\resources\tray.png`). It is used deliberately so that the menu item is recognizable as
"open this in CodeBuddy". CodeBuddy and its logo are trademarks of their respective owner; this extension
is an independent community project and is not affiliated with, or endorsed by, that owner.
