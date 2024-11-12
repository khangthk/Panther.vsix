using Microsoft.VisualStudio.Shell;
using Panther.Helpers;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace Panther.Settings
{
    internal class PantherOptionPage : DialogPage
    {
        private const string DisplayName = "PantherComponentsDir";
        private const string Description = "To set the path of the components directory, use this format: " +
                                           "[<path>\\Components\\ProSIM\\], and replace <path> with the absolute path.";

        private string _pantherComponentsDirBackup = string.Empty;

        [Category("Panther")]
        [DisplayName(DisplayName)]
        [Description(Description)]
        public string PantherComponentsDir { get; set; }

        protected override void OnApply(PageApplyEventArgs e)
        {
            if (string.IsNullOrEmpty(PantherComponentsDir?.Trim()))
            {
                PantherComponentsDir = string.Empty;
            }

            if (Path.IsPathRooted(PantherComponentsDir))
            {
                PantherComponentsDir = PantherComponentsDir.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            }

            base.OnApply(e);
        }

        public override void LoadSettingsFromStorage()
        {
            PantherComponentsDir = PantherHelper.GetPantherComponentsDirPath();
            _pantherComponentsDirBackup = PantherComponentsDir;
        }

        public override void SaveSettingsToStorage()
        {
            Environment.SetEnvironmentVariable(DisplayName, PantherComponentsDir, EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable(DisplayName, PantherComponentsDir, EnvironmentVariableTarget.Process);

            if (PantherComponentsDir != _pantherComponentsDirBackup)
            {
                MessageBox.Show("You have changed the value of PantherComponentDir.\nPlease restart Visual Studio to take effect.",
                                "Panther", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
