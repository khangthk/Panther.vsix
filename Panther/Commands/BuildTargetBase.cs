using Microsoft.VisualStudio.Shell;
using Panther.Helpers;
using System;
using System.Diagnostics;
using System.IO;

namespace Panther.Commands
{
    internal abstract class BuildTargetBase : CommandBase
    {
        private Stopwatch _stopwatch;

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

            CheckPantherComponentsDir();
            PrintCompleted();
        }

        public override bool IsEnabled()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return base.IsEnabled() && !PantherBuildEvents.Instance.IsBuilding &&
                   File.Exists(PantherHelper.GetPantherMSBuildTargetsFilePath());
        }

        protected void PrintCompleted()
        {
            _stopwatch.Stop();
            _ = OutputPaneHelper.OutputStringAsync($"========== Build completed at {DateTime.Now:hh:mm tt} and took {_stopwatch.Elapsed.TotalSeconds:F2} seconds ==========");
        }

        private void CheckPantherComponentsDir()
        {
            var pantherComponentsDir = PantherHelper.GetPantherComponentsDirPath();

            if (string.IsNullOrWhiteSpace(pantherComponentsDir))
            {
                _ = OutputPaneHelper.OutputStringAsync("[ERROR] <PantherComponentsDir> is not set. Go to [Panther -> Setting Components] in the menu to configure the path." + Environment.NewLine);
                return;
            }

            if (!Directory.Exists(pantherComponentsDir))
            {
                _ = OutputPaneHelper.OutputStringAsync("[ERROR] <PantherComponentsDir> is set, but the specified path does not exist. Go to [Panther -> Setting Components] in the menu to configure the path." + Environment.NewLine);
                return;
            }
        }
    }
}
