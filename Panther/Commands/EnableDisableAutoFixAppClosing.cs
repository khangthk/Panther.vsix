using Microsoft.VisualStudio.Shell;
using Panther.Helpers;
using System.IO;
using Task = System.Threading.Tasks.Task;

namespace Panther.Commands
{
    internal class EnableDisableAutoFixAppClosing : CommandBase
    {
        protected override async Task ExecuteAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            RegistryHelper.SaveAutoFixAppClosing(!RegistryHelper.GetAutoFixAppClosing());
        }

        public override string GetText() => MenuTexts.EnableDisableAutoFixAppClosing;

        public override bool IsEnabled()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return base.IsEnabled() && Directory.Exists(PantherHelper.GetBinDirPath());
        }

        public override bool IsChecked()
        {
            return RegistryHelper.GetAutoFixAppClosing();
        }
    }
}
