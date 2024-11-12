using Microsoft.VisualStudio.Shell;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Process = System.Diagnostics.Process;
using Task = System.Threading.Tasks.Task;

namespace Panther.Helpers
{
    enum Configuration { Debug, Release }
    enum Target { Dependencies, ThirdParty }

    internal static class MSBuildHelper
    {
        private static async Task BuildAsync(string arguments)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            string[] keys = { "MSBuildBinPath", "PlatformToolset" };
            var props = PantherHelper.GetProjectProperty(keys);
            var MSBuildBinPath = props[keys[0]];
            var PlatformToolset = props[keys[1]];

            if (string.IsNullOrEmpty(MSBuildBinPath) || string.IsNullOrEmpty(PlatformToolset))
            {
                OutputString($"--> $({keys[0]}) = {(!string.IsNullOrEmpty(MSBuildBinPath) ? MSBuildBinPath : "<not found>")}");
                OutputString($"--> $({keys[1]}) = {(!string.IsNullOrEmpty(PlatformToolset) ? PlatformToolset : "<not found>")}");
                return;
            }

            using (Process process = new Process())
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(MSBuildBinPath, "MSBuild.exe"),
                    Arguments = $"{arguments} /p:PlatformToolset={PlatformToolset}",
                    UseShellExecute = false,
                    ErrorDialog = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                };

                var valueSetting = PantherHelper.GetPantherComponentsDirPath();
                var valueProcess = process.StartInfo.EnvironmentVariables[PantherHelper.GetPantherComponentsDirName()];
                if (valueSetting != valueProcess)
                {
                    process.StartInfo.EnvironmentVariables[PantherHelper.GetPantherComponentsDirName()] = valueSetting;
                }

                process.StartInfo = startInfo;
                process.OutputDataReceived += OnOutputDataReceived;

                Debug.WriteLine($"--> Run: {startInfo.FileName} {startInfo.Arguments}");

                await Task.Run(() =>
                {
                    try
                    {
                        process.Start();
                        process.BeginOutputReadLine();
                        process.WaitForExit();
                    }
                    catch (Exception)
                    {
                    }
                });
            }
        }

        public static async Task BuildSolutionAsync(string solutionFilePath, Configuration cfg, bool rebuild = false)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            if (File.Exists(solutionFilePath))
            {
                var arguments = $"\"{solutionFilePath}\" /p:Configuration={(cfg == Configuration.Debug ? "Debug" : "Release")} {(rebuild ? "/t:Rebuild" : "")}";
                await BuildAsync(arguments);
            }
        }

        public static async Task BuildTargetAsync(Target target)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var filePath = PantherHelper.GetPantherMSBuildTargetsFilePath();
            if (File.Exists(filePath))
            {
                var arguments = $"\"{filePath}\" {(target == Target.Dependencies ? "/t:BuildDependencies" : "/t:BuildThirdParty")}";
                await BuildAsync(arguments);
            }
        }

        private static void OnOutputDataReceived(object proc, DataReceivedEventArgs ev)
        {
            OutputString(ev.Data);
        }

        private static void OutputString(string str)
        {
            _ = OutputPaneHelper.OutputStringAsync(str);
        }
    }
}
