using Microsoft.VisualStudio.Shell;
using Panther.Helpers;
using System.IO;
using System.Windows.Forms;

namespace Panther.Commands
{
    internal sealed class RunDICOMServer : RunCommandBase
    {
        public RunDICOMServer() : base(Exe.DICOMServer) { }

        protected override void OnCommandBegin()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            base.OnCommandBegin();

            var config = Path.Combine(PantherHelper.GetConfigurationsDirPath(), "Host_DICOMCommunicator.ini");
            if (!File.Exists(config))
            {
                MessageBox.Show($"Please run {Exe.DICOMCommunicator.ExeName} to configure before running {Exe.DICOMServer.ExeName} !!!",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Cancel = true;
            }
        }

        public override string GetText() => MenuTexts.RunDICOMServer;
    }
}
