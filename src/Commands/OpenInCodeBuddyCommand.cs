using EnvDTE;
using EnvDTE80;
using Microsoft;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace OpenInCodeBuddy
{
    /// <summary>
    /// Implements the two commands that hand the current solution/project/folder/file
    /// over to CodeBuddy.
    /// </summary>
    internal sealed class OpenInCodeBuddyCommand
    {
        private readonly Package _package;
        private readonly Options _options;

        private OpenInCodeBuddyCommand(Package package, Options options)
        {
            _package = package;
            _options = options;

            var commandService = (OleMenuCommandService)ServiceProvider.GetService(typeof(IMenuCommandService));

            if (commandService != null)
            {
                var menuCommandID = new CommandID(PackageGuids.guidOpenInCodeBuddyCmdSet, PackageIds.OpenInCodeBuddy);
                var menuItem = new MenuCommand(OpenSelectionInCodeBuddy, menuCommandID);
                commandService.AddCommand(menuItem);

                var currentCommandID = new CommandID(PackageGuids.guidOpenCurrentInCodeBuddyCmdSet, PackageIds.OpenCurrentInCodeBuddy);
                var currentItem = new MenuCommand(OpenCurrentFileInCodeBuddy, currentCommandID);
                commandService.AddCommand(currentItem);
            }
        }

        public static OpenInCodeBuddyCommand Instance { get; private set; }

        private IServiceProvider ServiceProvider
        {
            get { return _package; }
        }

        public static void Initialize(Package package, Options options)
        {
            Instance = new OpenInCodeBuddyCommand(package, options);
        }

        private void OpenCurrentFileInCodeBuddy(object sender, EventArgs e)
        {
            try
            {
                var dte = (DTE2)ServiceProvider.GetService(typeof(DTE));
                Assumes.Present(dte);

                var activeDocument = dte.ActiveDocument;

                if (activeDocument == null)
                {
                    MessageBox.Show("Couldn't find active document", Vsix.Name);
                    return;
                }

                var path = activeDocument.FullName;

                if (string.IsNullOrEmpty(path))
                {
                    MessageBox.Show("Couldn't resolve the folder", Vsix.Name);
                    return;
                }

                int line = 0;
                int column = 0;

                if (activeDocument.Selection is TextSelection selection)
                {
                    line = selection.ActivePoint.Line;
                    // note:
                    // LineCharOffset not DisplayColumn
                    // as it describes `buddycn -h`: -g --goto <file:line[:character]>
                    column = selection.ActivePoint.LineCharOffset;
                }

                OpenInCodeBuddy(path, line, column);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private void OpenSelectionInCodeBuddy(object sender, EventArgs e)
        {
            try
            {
                var dte = (DTE2)ServiceProvider.GetService(typeof(DTE));
                Assumes.Present(dte);

                string path = ProjectHelpers.GetSelectedPath(dte, _options.OpenSolutionProjectAsRegularFile);

                if (string.IsNullOrEmpty(path))
                {
                    MessageBox.Show("Couldn't resolve the folder", Vsix.Name);
                    return;
                }

                int line = 0;
                int column = 0;

                if (dte.ActiveDocument?.Selection is TextSelection selection)
                {
                    line = selection.ActivePoint.Line;
                    column = selection.ActivePoint.LineCharOffset;
                }

                OpenInCodeBuddy(path, line, column);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private void OpenInCodeBuddy(string path, int line = 0, int column = 0)
        {
            if (!EnsurePathExists())
                return;

            // Check if path contains multiple files (space-separated quoted paths)
            bool isMultipleFiles = path.Contains("\" \"");
            bool isDirectory = !isMultipleFiles && Directory.Exists(path);

            var args = isMultipleFiles
                ? path  // Multiple files are already properly quoted
                : isDirectory
                    ? "."
                    : line > 0
                        ? column > 0
                            ? $"-g \"{path}:{line}:{column}\""
                            : $"-g \"{path}:{line}\""
                        : $"\"{path}\"";

            if (!string.IsNullOrEmpty(_options.CommandLineArguments))
            {
                args = $"{args} {_options.CommandLineArguments}";
            }

            Launch(args, isDirectory ? path : null);
        }

        /// <summary>
        /// Starts CodeBuddy. The CLI (<c>resources\app\out\cli.js</c>) is preferred over the
        /// GUI executable because it understands the same arguments as <c>code</c>
        /// (<c>-g file:line:column</c> in particular) and hands the request over to the
        /// already running instance. Running it in Electron's node mode is exactly what
        /// <c>bin\buddycn.cmd</c> does, minus the console window.
        /// </summary>
        private void Launch(string arguments, string workingDirectory)
        {
            var exePath = _options.PathToExe;
            var cliPath = Path.Combine(Path.GetDirectoryName(exePath) ?? string.Empty, @"resources\app\out\cli.js");
            bool useCli = File.Exists(cliPath);

            var start = new System.Diagnostics.ProcessStartInfo
            {
                FileName = $"\"{exePath}\"",
                Arguments = useCli ? $"\"{cliPath}\" {arguments}" : arguments,
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
            };

            if (useCli)
            {
                start.EnvironmentVariables["ELECTRON_RUN_AS_NODE"] = "1";
                start.EnvironmentVariables.Remove("VSCODE_DEV");
            }

            if (!string.IsNullOrEmpty(workingDirectory))
            {
                start.WorkingDirectory = workingDirectory;
            }

            using (System.Diagnostics.Process.Start(start))
            {
            }
        }

        private bool EnsurePathExists()
        {
            if (File.Exists(_options.PathToExe))
                return true;

            var detected = CodeBuddyDetect.InRegistry()
                        ?? CodeBuddyDetect.InEnvVarPath()
                        ?? CodeBuddyDetect.InLocalAppData();

            if (!string.IsNullOrEmpty(detected))
            {
                SaveOptions(_options, detected);
                return true;
            }

            var box = MessageBox.Show(
                "I can't find CodeBuddy (CodeBuddy CN.exe). Would you like to help me find it?", Vsix.Name,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (box == DialogResult.No)
                return false;

            var dialog = new OpenFileDialog
            {
                DefaultExt = ".exe",
                FileName = "CodeBuddy CN.exe",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                CheckFileExists = true
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SaveOptions(_options, dialog.FileName);
                return true;
            }

            return false;
        }

        private void SaveOptions(Options options, string path)
        {
            options.PathToExe = path;
            options.SaveSettingsToStorage();
        }
    }

    /// <summary>
    /// Locates the CodeBuddy installation on this machine.
    /// </summary>
    internal static class CodeBuddyDetect
    {
        // Shell verbs registered by the CodeBuddy installer, e.g.
        // HKCU\SOFTWARE\Classes\*\shell\CodeBuddy CN -> Icon = ...\CodeBuddy CN.exe
        private static readonly string[] RegistryKeys =
        {
            @"SOFTWARE\Classes\*\shell\CodeBuddy CN",
            @"SOFTWARE\Classes\*\shell\CodeBuddy",
            @"SOFTWARE\Classes\*\shell\CodeBuddyCN",
        };

        private static readonly string[] ExecutableNames =
        {
            "CodeBuddy CN.exe",
            "CodeBuddy.exe",
            "CodeBuddyCN.exe",
        };

        // Installer layouts, relative to %LOCALAPPDATA% (and to each fixed drive root)
        private static readonly string[] RelativeInstallFolders =
        {
            @"Programs\CodeBuddy CN",
            @"Programs\CodeBuddy",
            @"Programs\CodeBuddyCN",
        };

        private static readonly string[] CliShimNames =
        {
            "buddycn.cmd",
            "buddycn.exe",
        };

        internal static string InRegistry()
        {
            foreach (var keyName in RegistryKeys)
            {
                try
                {
                    using (var subKey = Registry.CurrentUser.OpenSubKey(keyName))
                    {
                        var value = subKey?.GetValue("Icon") as string;

                        if (string.IsNullOrEmpty(value))
                            continue;

                        var path = Environment.ExpandEnvironmentVariables(value.Trim().Trim('"'));

                        if (File.Exists(path))
                            return path;
                    }
                }
                catch
                {
                    // Ignore unreadable keys and keep probing
                }
            }

            return null;
        }

        internal static string InEnvVarPath()
        {
            var envPath = Environment.GetEnvironmentVariable("PATH");

            if (string.IsNullOrEmpty(envPath))
                return null;

            foreach (var dir in envPath.Split(';'))
            {
                if (string.IsNullOrWhiteSpace(dir))
                    continue;

                var found = FindInFolder(dir);

                if (!string.IsNullOrEmpty(found))
                    return found;

                if (!HasCliShim(dir))
                    continue;

                // bin\buddycn.cmd -> the executable lives one level up
                var parent = Path.GetDirectoryName(dir.TrimEnd('\\'));
                found = FindInFolder(parent);

                if (!string.IsNullOrEmpty(found))
                    return found;
            }

            return null;
        }

        internal static string InLocalAppData()
        {
            var localAppData = Environment.GetEnvironmentVariable("LOCALAPPDATA");

            foreach (var relativeFolder in RelativeInstallFolders)
            {
                if (!string.IsNullOrEmpty(localAppData))
                {
                    var found = FindInFolder(Path.Combine(localAppData, relativeFolder));

                    if (!string.IsNullOrEmpty(found))
                        return found;
                }

                // CodeBuddy installed on a drive other than the one holding %LOCALAPPDATA%
                foreach (var drive in DriveInfo.GetDrives())
                {
                    if (drive.DriveType != DriveType.Fixed)
                        continue;

                    var found = FindInFolder(Path.Combine(drive.RootDirectory.FullName, relativeFolder));

                    if (!string.IsNullOrEmpty(found))
                        return found;
                }
            }

            return null;
        }

        private static string FindInFolder(string folder)
        {
            if (string.IsNullOrEmpty(folder))
                return null;

            foreach (var name in ExecutableNames)
            {
                try
                {
                    var path = Path.Combine(folder, name);

                    if (File.Exists(path))
                        return path;
                }
                catch
                {
                    // Invalid characters in PATH entries
                }
            }

            return null;
        }

        private static bool HasCliShim(string folder)
        {
            foreach (var name in CliShimNames)
            {
                try
                {
                    if (File.Exists(Path.Combine(folder, name)))
                        return true;
                }
                catch
                {
                    // Invalid characters in PATH entries
                }
            }

            return false;
        }
    }
}
