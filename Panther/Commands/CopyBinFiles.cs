using Microsoft.VisualStudio.Shell;
using Panther.Helpers;
using Panther.Windows;
using System.IO;
using System.Windows;
using Task = System.Threading.Tasks.Task;

namespace Panther.Commands
{
    internal class CopyBinFiles : CommandBase
    {
        protected override async Task ExecuteAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            if (RegistryHelper.GetAutoUnlockUtility())
            {
                UnlockUtilityLockedByUser.UnlockUtility();
            }

            if (RegistryHelper.GetAutoFixAppClosing())
            {
                FixAppClosingAfter15Seconds.FixApp();
            }

            _ = new CopyBinFilesWindow() { Owner = Application.Current?.MainWindow }.ShowDialog();
        }

        public override string GetText() => MenuTexts.CopyBinFiles;

        public override bool IsEnabled()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return base.IsEnabled() && Directory.Exists(PantherHelper.GetBinDirPath());
        }
    }
}
