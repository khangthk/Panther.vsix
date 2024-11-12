using Microsoft.VisualStudio.Shell;
using Panther.Helpers;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Task = System.Threading.Tasks.Task;

namespace Panther.Commands
{
    internal struct ExeInfo
    {
        public string ExeName { get; set; }
        public string[] Arguments { get; set; }
        public ExeInfo(string ExeName, string[] Arguments)
        {
            this.ExeName = ExeName;
            this.Arguments = Arguments;
        }
    }

    internal static class Exe
    {
        public static ExeInfo AMSManager = new ExeInfo("AMSManager.exe", null);
        public static ExeInfo ConfigTool = new ExeInfo("ConfigTool.exe", null);
        public static ExeInfo DICOMCommunicator = new ExeInfo("DICOMCommunicator.exe", null);
        public static ExeInfo DICOMServer = new ExeInfo("DICOMServer.exe", new string[] { "-console" });
        public static ExeInfo ImportTool = new ExeInfo("ImportTool.exe", null);
        public static ExeInfo LicenseManager = new ExeInfo("LicenseManager.exe", null);
        public static ExeInfo LicenseServer = new ExeInfo("LicenseServer.exe", new string[] { "-console" });
        public static ExeInfo LoggingTool = new ExeInfo("LoggingTool.exe", null);
        public static ExeInfo MachineDataEntry = new ExeInfo("MachineDataEntry.exe", null);
        public static ExeInfo PantherTPS = new ExeInfo("PantherTPS.exe", null);
        public static ExeInfo SecurityManager = new ExeInfo("SecurityManager.exe", null);
        public static ExeInfo WTCP = new ExeInfo("WTCP.exe", null);
    }

    internal abstract class RunCommandBase : CommandBase
    {
        private ExeInfo _exeInfo;

        protected RunCommandBase(ExeInfo exeInfo) => _exeInfo = exeInfo;

        protected override async Task ExecuteAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            if (RegistryHelper.GetAutoDeleteLockFiles())
            {
                DeleteLockFilesInPantherSite.DeleteLockFiles();
            }

            RunExe();
        }

        public override bool IsEnabled()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var parent = PantherHelper.GetBinDirPath();
            return base.IsEnabled() && !string.IsNullOrEmpty(parent) && File.Exists(Path.Combine(parent, _exeInfo.ExeName));
        }

        private void RunExe()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (string.IsNullOrEmpty(_exeInfo.ExeName))
            {
                return;
            }

            var args = _exeInfo.Arguments != null && _exeInfo.Arguments.Length > 0 ? string.Join(" ", _exeInfo.Arguments) : string.Empty;
            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c start /d \"{PantherHelper.GetBinDirPath()}\" {_exeInfo.ExeName} {args}",
                CreateNoWindow = true,
                UseShellExecute = false,
                ErrorDialog = false
            };

            try
            {
                Debug.WriteLine($"--> Run: {startInfo.FileName} {startInfo.Arguments}");
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to run: {startInfo.FileName} {startInfo.Arguments}\n\nError: {ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
