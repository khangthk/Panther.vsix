using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using Task = System.Threading.Tasks.Task;

namespace Panther.Helpers
{
    internal static class OutputPaneHelper
    {
        public static void ActivatePantherOutputPane()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            GetPantherOutputPane().Activate();
            ActivateOutputWindow();
        }

        public static void ClearPantherOutputPane()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            GetPantherOutputPane().Clear();
        }

        public static async Task OutputStringAsync(string msg)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            GetPantherOutputPane().OutputString(msg + Environment.NewLine);
        }

        private static OutputWindowPane GetPantherOutputPane()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return GetOutputPane("Panther");
        }

        private static OutputWindowPane GetOutputPane(string name)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            OutputWindowPane outputPane;
            OutputWindowPanes panes = PackageHelper.GetDTE().ToolWindows.OutputWindow.OutputWindowPanes;

            try
            {
                outputPane = panes.Item(name);
            }
            catch (Exception)
            {
                outputPane = panes.Add(name);
            }

            return outputPane;
        }

        private static void ActivateOutputWindow()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Window outputWindow = PackageHelper.GetDTE().Windows.Item(Constants.vsWindowKindOutput);
            if (outputWindow != null)
            {
                outputWindow.IsFloating = false;
                outputWindow.Visible = true;
                outputWindow.Activate();
            }
        }
    }
}
