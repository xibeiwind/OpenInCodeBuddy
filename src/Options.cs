using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace OpenInCodeBuddy
{
    public class Options : DialogPage
    {
        [Category("General")]
        [DisplayName("Command line arguments")]
        [Description("Command line arguments to pass to the CodeBuddy CLI")]
        public string CommandLineArguments { get; set; }

        [Category("General")]
        [DisplayName("Path to CodeBuddy executable")]
        [Description("Specify the path to CodeBuddy CN.exe.")]
        public string PathToExe { get; set; } = Environment.ExpandEnvironmentVariables(@"%localappdata%\Programs\CodeBuddy CN\CodeBuddy CN.exe");

        [Category("General")]
        [DisplayName("Open solution/project as regular file")]
        [Description("When true, opens solutions/projects as regular files and does not load folder path into CodeBuddy.")]
        public bool OpenSolutionProjectAsRegularFile { get; set; }

        protected override void OnApply(PageApplyEventArgs e)
        {
            if (!File.Exists(PathToExe))
            {
                e.ApplyBehavior = ApplyKind.Cancel;
                MessageBox.Show($"The file \"{PathToExe}\" doesn't exist.", Vsix.Name, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            base.OnApply(e);
        }
    }
}
