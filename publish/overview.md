# Open in CodeBuddy

Adds a menu command that opens any solution, project, folder or file from Visual Studio in **CodeBuddy** —
the VS Code-based AI IDE — including CodeBuddy CN.

A port of [OpenInVsCode](https://github.com/madskristensen/OpenInVsCode): same commands, same menus,
same options, retargeted to CodeBuddy.

## Features

- **Solution Explorer context menu** — right-click a solution, project, folder or file and choose
  *Open in CodeBuddy*:
  - a solution opens its folder,
  - a project opens its root folder,
  - a folder or file opens itself,
  - multiple selected files are passed as multiple arguments.
- **Menu command + keyboard shortcut** — *Open in CodeBuddy* in the main menu opens the active document
  and carries the cursor position, so CodeBuddy jumps to the same line and column. Default shortcut:
  `Ctrl+Shift+Y`.
- **Automatic discovery** of the CodeBuddy installation:
  1. the `CodeBuddy CN` shell verb registered by the installer,
  2. the `buddycn` CLI shim on `PATH`,
  3. the usual per-user install folders on any fixed drive,
  4. otherwise a file dialog asks you to point at `CodeBuddy CN.exe`.

## Options

**Tools → Options → Web → Open in CodeBuddy**

| Option | Description |
|---|---|
| Path to CodeBuddy executable | Full path to `CodeBuddy CN.exe` |
| Command line arguments | Extra arguments appended to every launch |
| Open solution/project as regular file | Open the `.sln` / `.csproj` file itself instead of its folder |

## How it launches CodeBuddy

The extension prefers the CodeBuddy CLI (`resources\app\out\cli.js` next to the executable), started in
Electron's node mode (`ELECTRON_RUN_AS_NODE=1`) — exactly what the bundled `bin\buddycn.cmd` does, minus the
console window. This gives full `code`-compatible argument handling, including `-g file:line:column`.
If `cli.js` is not found, the GUI executable is started directly.

No console window is shown and Visual Studio is never blocked.

## Requirements

- Visual Studio 2017, 2019, 2022 (amd64 / arm64) or 2026
- CodeBuddy installed locally

## Notes

- This is an independent community extension. It is not affiliated with, or endorsed by, the CodeBuddy
  product or its vendor; the icon is derived from the CodeBuddy logo only to indicate which application
  the command opens.
- All Visual Studio instances must be closed while installing or updating the extension.
- Failures are logged to the *Open in CodeBuddy* pane of the Output window instead of interrupting you
  with dialogs.

## 中文说明

在 Visual Studio 里右键任意解决方案、项目、文件夹或文件，即可用 CodeBuddy 打开；
`Ctrl+Shift+Y` 打开当前文件并定位到光标行。CodeBuddy 装在非默认路径时，
可在 **工具 → 选项 → Web → Open in CodeBuddy** 中指定可执行文件路径。

## License

MIT
