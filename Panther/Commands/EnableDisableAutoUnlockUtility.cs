using Microsoft.VisualStudio.Shell;
using Panther.Helpers;
using System.IO;
using Task = System.Threading.Tasks.Task;

namespace Panther.Commands
{
    internal class EnableDisableAutoUnlockUtility : CommandBase
    {
        protected override async Task ExecuteAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            RegistryHelper.SaveAutoUnlockUtility(!RegistryHelper.GetAutoUnlockUtility());
        }

        public override string GetText() => MenuTexts.EnableDisableAutoUnlockUtility;

        public override bool IsEnabled()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return base.IsEnabled() && Directory.Exists(PantherHelper.GetConfigurationsDirPath());
        }

        public override bool IsChecked()
        {
            return RegistryHelper.GetAutoUnlockUtility();
        }
    }
}
