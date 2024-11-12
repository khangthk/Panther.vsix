using Microsoft.VisualStudio.Shell;
using Panther.Helpers;
using System.IO;
using Task = System.Threading.Tasks.Task;

namespace Panther.Commands
{
    internal class UnlockUtilityLockedByUser : CommandBase
    {
        protected override async Task ExecuteAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            UnlockUtility();
        }

        public override string GetText() => MenuTexts.UnlockUtilityLockedByUser;

        public override bool IsEnabled()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return base.IsEnabled() && Directory.Exists(PantherHelper.GetConfigurationsDirPath());
        }

        public static void UnlockUtility()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var filePath = PantherHelper.GetConfigurationsFilePath();
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            IniHelper.DeleteSection(filePath, $"[{fileName}\\ProSIM\\WindowsUser]");
        }
    }
}
