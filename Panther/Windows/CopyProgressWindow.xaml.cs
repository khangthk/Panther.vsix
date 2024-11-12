using Panther.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Panther.Windows
{
    public partial class CopyProgressWindow : Window
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly CopyProgressViewModel _viewModel;
        private readonly List<FileItem> _selectedItems;
        private readonly string _srcBin;
        private readonly string _dstBin;
        private int _total;
        private int _copied;
        private int _failed;
        private int _processed;
        private bool _pause;
        private Task _copyTask;
        private Stopwatch _stopwatch;

        public CopyProgressWindow(List<FileItem> selectedItems, string srcBin, string dstBin)
        {
            InitializeComponent();
            _cancellationTokenSource = new CancellationTokenSource();
            _viewModel = new CopyProgressViewModel();
            _selectedItems = selectedItems;
            _srcBin = srcBin;
            _dstBin = dstBin;
            _total = 0;
            _copied = 0;
            _failed = 0;
            _processed = 0;
            _pause = false;
            DataContext = _viewModel;
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ConfigureWindow();
            _stopwatch = Stopwatch.StartNew();
            _copyTask = StartCopyAsync();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (_copyTask != null && !_copyTask.IsCompleted && _processed != _total)
            {
                _pause = true;
                _stopwatch.Stop();
                var result = MessageBoxEx.Show(this, "Are you sure you want to cancel the copy operation?",
                                               Title, MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    _cancellationTokenSource.Cancel();
                    PrintCompleted();
                    e.Cancel = false;
                }
                else
                {
                    _stopwatch.Start();
                    _pause = false;
                    e.Cancel = true;
                }
            }
            else
            {
                base.OnClosing(e);
            }
        }

        private void OnCancelOrCloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private void ConfigureWindow()
        {
            const int GWL_STYLE = -16;
            const int WS_MINIMIZEBOX = 0x20000;
            const int WS_MAXIMIZEBOX = 0x10000;

            var hwnd = new WindowInteropHelper(this).Handle;
            var style = GetWindowLong(hwnd, GWL_STYLE);
            style &= ~WS_MINIMIZEBOX & ~WS_MAXIMIZEBOX;
            SetWindowLong(hwnd, GWL_STYLE, style);
        }

        private async Task StartCopyAsync()
        {
            try
            {
                _viewModel.ButtonText = "Cancel";
                await ExecuteCopyAsync();
                _viewModel.CurrentFileName = $"Total files: {_total}  Copied: {_copied}  Failed: {_failed}";
                _viewModel.ButtonText = "Close";
            }
            finally
            {
                PrintCompleted();
            }
        }

        private async Task ExecuteCopyAsync()
        {
            _total = await CalculateTotalFilesAsync();

            foreach (var item in _selectedItems)
            {
                await (item.IsFolder ? CopyFolderAsync(item) : CopyFileAsync(item));
            }
        }

        private async Task<int> CalculateTotalFilesAsync()
        {
            return await Task.Run(() => CalculateTotalFiles());
        }

        private int CalculateTotalFiles()
        {
            int totalFiles = 0;

            foreach (var item in _selectedItems)
            {
                totalFiles += item.IsFolder ? Directory.GetFiles(item.Path, "*.*", SearchOption.AllDirectories).Length : 1;
            }

            return totalFiles;
        }

        private async Task CopyFolderAsync(FileItem folderItem)
        {
            var files = Directory.GetFiles(folderItem.Path, "*.*", SearchOption.AllDirectories);

            foreach (var filePath in files)
            {
                var relativePath = filePath.Substring(_srcBin.Length + 1);
                var dstFilePath = Path.Combine(_dstBin, relativePath);

                await CopyFileToDestinationAsync(filePath, dstFilePath);
            }
        }

        private async Task CopyFileAsync(FileItem fileItem)
        {
            var dstFilePath = Path.Combine(_dstBin, fileItem.Name);

            await CopyFileToDestinationAsync(fileItem.Path, dstFilePath);
        }

        private async Task CopyFileToDestinationAsync(string srcFilePath, string dstFilePath)
        {
            if (_pause)
            {
                while (_pause)
                {
                    await Task.Delay(100);
                }
            }

            _cancellationTokenSource.Token.ThrowIfCancellationRequested();

            var fileName = Path.GetFileName(srcFilePath);
            _viewModel.CurrentFileName = fileName;

            try
            {
                _viewModel.Progress = (++_processed / (double)_total) * 100;

                await Task.Run(() => Directory.CreateDirectory(Path.GetDirectoryName(dstFilePath)), _cancellationTokenSource.Token);
                await Task.Run(() => File.Copy(srcFilePath, dstFilePath, true), _cancellationTokenSource.Token);
                await OutputPaneHelper.OutputStringAsync($"[{_processed}/{_total}][OK] {fileName} => {dstFilePath}");

                _copied++;
            }
            catch (Exception ex)
            {
                await OutputPaneHelper.OutputStringAsync($"[{_processed}/{_total}][ERROR] {fileName} => {ex.Message}");

                if (_failed++ == 0)
                {
                    FileCopyProgressBar.Foreground = new SolidColorBrush(Colors.OrangeRed);
                }
            }
        }

        private void PrintCompleted()
        {
            _stopwatch.Stop();
            _ = OutputPaneHelper.OutputStringAsync($"========== Total files: {_total}  Copied: {_copied}  Failed: {_failed} ==========");
            _ = OutputPaneHelper.OutputStringAsync($"========== Copy completed at {DateTime.Now:hh:mm tt} and took {_stopwatch.Elapsed.TotalSeconds:F2} seconds ==========");
        }
    }
}
