# 在 Visual Studio 里一键用 CodeBuddy 打开

在 Visual Studio 的解决方案资源管理器里右键，或按 `Ctrl+Shift+Y`，直接用 **CodeBuddy** 打开你正在看的
解决方案、项目、文件夹或文件——光标停在哪一行，CodeBuddy 就跳到哪一行。

适合这样的场景：平时在 Visual Studio 里编译调试，但看代码、让 AI 改代码更习惯在 CodeBuddy 里做。
不用再翻资源管理器找路径、复制粘贴，一次点击就过去。

## 功能

| 操作位置 | 效果 |
|---|---|
| 右键**解决方案** | 在 CodeBuddy 中打开解决方案所在目录 |
| 右键**项目** | 打开项目根目录 |
| 右键**文件夹 / 文件** | 打开该文件夹或该文件 |
| 多选若干文件后右键 | 一次以多个参数传入 |
| 菜单 **Open in CodeBuddy** 或 `Ctrl+Shift+Y` | 打开当前文档，并定位到光标所在行列 |

## 快捷键

| 命令 | 默认快捷键 |
|---|---|
| 打开当前文档（带光标位置） | `Ctrl+Shift+Y` |

可在 **工具 → 选项 → 环境 → 键盘** 中自行改绑。

## 选项

**工具 → 选项 → Web → Open in CodeBuddy**

| 选项 | 说明 |
|---|---|
| Path to CodeBuddy executable | `CodeBuddy CN.exe` 的完整路径；首次找不到时会自动探测并写入 |
| Command line arguments | 每次启动 CodeBuddy 时追加的参数，例如 `--new-window` |
| Open solution/project as regular file | 打开 `.sln` / `.csproj` 文件本身，而不是它所在的文件夹 |

## 关于 CodeBuddy 的安装位置

扩展会依次自动查找：

1. 安装程序注册的 `CodeBuddy CN` 右键菜单项（`HKCU\Software\Classes\*\shell\CodeBuddy CN`）
2. `PATH` 上的 `buddycn` 命令行垫片
3. 常见安装目录（`%LOCALAPPDATA%\Programs\CodeBuddy CN` 及任意固定磁盘上的同类目录）

都找不到时会弹框请你手动指定 `CodeBuddy CN.exe`，之后会记住这个路径。装在非默认盘、绿色版也能用。

## 支持版本

- Visual Studio 2017 / 2019 / 2022（amd64、arm64）/ 2026
- 需要本机已安装 CodeBuddy（含 CodeBuddy CN）

## 安装与更新

- 安装或更新扩展前，请**关闭所有 Visual Studio 窗口**，否则安装程序会提示无法写入。
- 更新后的新版本会在 Visual Studio 的「管理扩展」中提示，更新同样需要先关闭 Visual Studio。

## 常见问题

**装了但右键菜单里没有？**
先确认「管理扩展」里该扩展处于启用状态；新装扩展需要重启 Visual Studio 才会加载。

**点了没反应？**
打开 **视图 → 输出**，把下拉框切到 *Open in CodeBuddy*，失败原因会记在那里（例如路径失效）。
也可以在选项页里重新指定可执行文件路径。

**跳行位置不准？**
以 Visual Studio 的行号与字符偏移为准；若文件在 CodeBuddy 中有格式差异（如不同换行符），可能会有偏差。

**会把我打开的代码传出去吗？**
不会。该扩展只在本机启动 CodeBuddy 进程并传递文件路径，不联网、不上传任何代码，也不改动你的文件。

## 说明

本扩展是独立社区项目，与 CodeBuddy 产品的厂商无隶属关系，也未获其背书。图标取自 CodeBuddy 自带
logo，仅用于标示「该命令会打开 CodeBuddy」。

## 许可证

MIT
