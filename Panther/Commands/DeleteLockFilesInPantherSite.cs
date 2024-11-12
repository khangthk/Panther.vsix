using Microsoft.VisualStudio.Shell;
using Panther.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Task = System.Threading.Tasks.Task;

namespace Panther.Commands
{
    internal class DeleteLockFilesInPantherSite : CommandBase
    {
        private readonly string _patientKey = "Patients";
        private readonly string _machineKey = "Machine";
        private readonly string _patientLockFile = "!PatientLocked.dat";
        private readonly string _machineLockFile = "!MachineLocked.dat";
        private int _resultPatientSuccess;
        private int _resultPatientFailed;
        private int _resultMachineSuccess;
        private int _resultMachineFailed;
        private Stopwatch _stopwatch;
        private static DeleteLockFilesInPantherSite _instance;

        public DeleteLockFilesInPantherSite() : base() { _instance = this; }

        protected override async Task ExecuteAsync()
        {
            await DeleteLockFilesAsync();
        }

        protected override void OnCommandBegin()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            base.OnCommandBegin();

            OutputPaneHelper.ActivatePantherOutputPane();
            OutputPaneHelper.ClearPantherOutputPane();

            _stopwatch = Stopwatch.StartNew();
        }

        protected override void OnCommandDone()
        {
            base.OnCommandDone();
            PrintCompleted();
        }

        public override string GetText() => MenuTexts.DeleteLockFilesInPantherSite;

        public override bool IsEnabled()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return base.IsEnabled() && Directory.Exists(PantherHelper.GetConfigurationsDirPath());
        }

        public static void DeleteLockFiles()
        {
            _ = _instance.DeleteLockFilesAsync(false);
        }

        private async Task DeleteLockFilesAsync(bool showResultInOutputPane = true)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var patientDirPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var machineDirPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var users = GetUsers();

            foreach (var user in users)
            {
                var filePath = Path.Combine(PantherHelper.GetConfigurationsDirPath(), user + ".ini");
                var result = IniHelper.GetIniValue(filePath, $"[{user}\\ProSIM\\Folders]", new string[] { _patientKey, _machineKey });
                if (result != null && !string.IsNullOrEmpty(result[_patientKey]))
                {
                    patientDirPaths.Add(result[_patientKey]);
                    if (showResultInOutputPane)
                    {
                        await OutputPaneHelper.OutputStringAsync($"{user}|Patients = {(!string.IsNullOrEmpty(result[_patientKey]) ? result[_patientKey] : "<not found>")}");
                    }
                }
                if (result != null && !string.IsNullOrEmpty(result[_patientKey]))
                {
                    machineDirPaths.Add(result[_machineKey]);
                    if (showResultInOutputPane)
                    {
                        await OutputPaneHelper.OutputStringAsync($"{user}|Machines = {(!string.IsNullOrEmpty(result[_machineKey]) ? result[_machineKey] : "<not found>")}");
                    }
                }
            }

            var resultPatient = await Task.WhenAll(patientDirPaths.Select(dir => FileFolderHelper.DeleteFileAsync(dir, _patientLockFile, true, showResultInOutputPane)));
            var resultMachine = await Task.WhenAll(machineDirPaths.Select(dir => FileFolderHelper.DeleteFileAsync(dir, _machineLockFile, true, showResultInOutputPane)));
            _resultPatientSuccess = resultPatient.Sum(result => result.success);
            _resultPatientFailed = resultPatient.Sum(result => result.failed);
            _resultMachineSuccess = resultMachine.Sum(result => result.success);
            _resultMachineFailed = resultMachine.Sum(result => result.failed);
        }

        private static List<string> GetUsers()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var users = new List<string> { "DefaultUser" };
            var filePath = PantherHelper.GetConfigurationsFilePath();
            var loginName = IniHelper.GetIniValue(filePath, $"[{Path.GetFileNameWithoutExtension(filePath)}\\ProSIM\\General]", "LastLogin");
            if (!string.IsNullOrEmpty(loginName))
            {
                users.Add(loginName);
            }

            return users;
        }

        protected void PrintCompleted()
        {
            _stopwatch.Stop();
            _ = OutputPaneHelper.OutputStringAsync($"========== [{_patientLockFile}] Total files: {_resultPatientSuccess + _resultPatientFailed}  Deleted: {_resultPatientSuccess}  Failed: {_resultPatientFailed} ==========");
            _ = OutputPaneHelper.OutputStringAsync($"========== [{_machineLockFile}] Total files: {_resultMachineSuccess + _resultMachineFailed}  Deleted: {_resultMachineSuccess}  Failed: {_resultMachineFailed} ==========");
            _ = OutputPaneHelper.OutputStringAsync($"========== Delete completed at {DateTime.Now:hh:mm tt} and took {_stopwatch.Elapsed.TotalSeconds:F2} seconds ==========");
        }
    }
}
