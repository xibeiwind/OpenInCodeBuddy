# Open in CodeBuddy

[English](README.md) | 简体中文

一个 Visual Studio 扩展：添加菜单命令，让你在 **CodeBuddy**（含 CodeBuddy CN）中打开任意解决方案、项目、文件夹或文件。

本项目参考 Mads Kristensen 的 [OpenInVsCode](https://github.com/madskristensen/OpenInVsCode)，目标应用改为 CodeBuddy。

## 功能

- 在**解决方案资源管理器**中右键任意解决方案、项目、文件夹或文件 → **Open in CodeBuddy**
  - 解决方案打开它所在的目录，项目打开项目根目录，文件夹与文件打开自身
  - 多选文件时作为多个文件参数一次传入
- 主菜单 **Open in CodeBuddy** 打开当前活动文档，并带上光标位置
  - 默认快捷键：`Ctrl+Shift+Y`
- CodeBuddy 不在默认安装位置时自动探测：
  1. 安装程序注册的 `CodeBuddy CN` shell 动词（`HKCU\...\Classes\*\shell\CodeBuddy CN`）
  2. `PATH` 上的 `buddycn` 命令行垫片
  3. 任意固定磁盘上的常见安装目录
  最后仍找不到时，弹文件选择框让你手动指定 `CodeBuddy CN.exe`。

## 选项

**工具 → 选项 → Web → Open in CodeBuddy**：

| 选项 | 说明 |
|---|---|
| Path to CodeBuddy executable | `CodeBuddy CN.exe` 的完整路径 |
| Command line arguments | 每次启动时追加的命令行参数 |
| Open solution/project as regular file | 打开 `.sln`/`.csproj` 文件本身，而不是其所在文件夹 |

## 启动方式

扩展优先调用 CodeBuddy 的命令行（可执行文件旁的 `resources\app\out\cli.js`），以 Electron 的 node 模式
（`ELECTRON_RUN_AS_NODE=1`）启动——与自带的 `bin\buddycn.cmd` 完全等价，只是不会闪控制台窗口。这样能获得
与 `code` 完全一致的参数语义，包括用 `-g file:line:column` 跳转到光标所在行列。找不到 `cli.js` 时，直接
启动 GUI 可执行文件。

## 系统要求

- Visual Studio 2017、2019、2022（amd64/arm64）或 2026
- 本机已安装 CodeBuddy

## 构建

```cmd
msbuild OpenInCodeBuddy.sln /t:Restore;Build /p:Configuration=Release /p:DeployExtension=false
```

产物位于 `src/bin/Release/OpenInCodeBuddy.vsix`——双击安装，或在 Visual Studio 中按 `F5` 到实验实例调试。

## 发布到应用市场

1. publisher 已配置为 **`xibeiwind`**，出现在 `src/source.extension.vsixmanifest`（`Identity Publisher`）与
   [`publish/vs-publish.json`](publish/vs-publish.json) 两处。若你 fork 后以其他 publisher 发布，两处都要改。
2. 在 Azure DevOps 创建 Personal Access Token，组织选 *All accessible organizations*，作用域勾
   **Marketplace → Manage**。
3. 在三个位置同步升版本（`vsixmanifest`、`source.extension.cs`、`AssemblyInfo.cs`），重新构建后执行：

```cmd
"C:\Program Files\Microsoft Visual Studio\2022\Enterprise\VSSDK\VisualStudioIntegration\Tools\Bin\VsixPublisher.exe" publish ^
  -payload "src\bin\Release\OpenInCodeBuddy.vsix" ^
  -publishManifest "publish\vs-publish.json" ^
  -personalAccessToken "%MARKETPLACE_PAT%"
```

4. 到应用市场管理页确认——命令行上传后不一定是公开状态，需要在管理页设为公开。

## 许可证

[MIT](LICENSE)

扩展图标取自 CodeBuddy 安装目录中的 logo（`resources\app\resources\tray.png`），有意保留以便用户一眼识别
「这个菜单会用 CodeBuddy 打开」。CodeBuddy 及其 logo 为相应权利人的商标；本扩展是独立社区项目，与该权利人
无隶属关系，也未获其背书。
