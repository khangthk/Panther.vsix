using Microsoft.VisualStudio.Shell;
using Panther.Helpers;
using System.IO;
using Task = System.Threading.Tasks.Task;

namespace Panther.Commands
{
    internal class EnableDisableAutoDeleteLockFiles : CommandBase
    {
        protected override async Task ExecuteAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            RegistryHelper.SaveAutoDeleteLockFiles(!RegistryHelper.GetAutoDeleteLockFiles());
        }

        public override string GetText() => MenuTexts.EnableDisableAutoDeleteLockFiles;

        public override bool IsEnabled()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return base.IsEnabled() && Directory.Exists(PantherHelper.GetConfigurationsDirPath());
        }

        public override bool IsChecked()
        {
            return RegistryHelper.GetAutoDeleteLockFiles();
        }
    }
}
