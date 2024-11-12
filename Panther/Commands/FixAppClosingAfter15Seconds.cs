using Microsoft.VisualStudio.Shell;
using Panther.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using Task = System.Threading.Tasks.Task;

namespace Panther.Commands
{
    internal class FixAppClosingAfter15Seconds : CommandBase
    {
        private static readonly List<string> _libs = new List<string>
        {
            "Base",
            "CCCS Calculation",
            "CCCS_GPU_Calculation",
            "CoreCommon",
            "GK Optimization",
            "IMRT Optimization",
            "PantherCommon",
            "Probdom",
            "Resources",
            "StandardUI"
        };

        protected override async Task ExecuteAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            FixApp();
        }

        public override string GetText() => MenuTexts.FixAppClosingAfter15Seconds;

        public override bool IsEnabled()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return base.IsEnabled() && Directory.Exists(PantherHelper.GetBinDirPath());
        }

        public static void FixApp()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            UpdateDllWriteTime(true);
            UpdateDllWriteTime(false);
        }

        private static void UpdateDllWriteTime(bool isDebug)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var maxTime = GetMaxWriteTime(_libs, isDebug);
            var minTime = maxTime.AddMinutes(-50);

            foreach (var lib in _libs)
            {
                try
                {
                    var path = Path.Combine(PantherHelper.GetBinDirPath(), lib + (isDebug ? "D" : "") + ".dll");
                    var time = File.GetLastWriteTime(path);
                    if (time < minTime)
                    {
                        File.SetLastWriteTime(path, minTime);
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private static DateTime GetMaxWriteTime(List<string> libs, bool isDebug)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            DateTime max = DateTime.MinValue;
            foreach (var lib in libs)
            {
                try
                {
                    var path = Path.Combine(PantherHelper.GetBinDirPath(), lib + (isDebug ? "D" : "") + ".dll");
                    var time = File.GetLastWriteTime(path);
                    max = DateTime.Compare(max, time) > 0 ? max : time;
                }
                catch (Exception)
                {
                }
            }

            return max;
        }
    }
}
