using Microsoft.VisualStudio.Shell;
using Panther.Helpers;
using Panther.Settings;
using Task = System.Threading.Tasks.Task;

namespace Panther.Commands
{
    internal class SettingComponents : CommandBase
    {
        protected override async Task ExecuteAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            PackageHelper.GetPackage().ShowOptionPage(typeof(PantherOptionPage));
        }

        public override string GetText() => MenuTexts.SettingComponents;

        public override bool IsEnabled()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return base.IsEnabled() && PantherHelper.IsPantherSolution();
        }
    }
}
