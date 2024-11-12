using Microsoft.VisualStudio.Shell;
using Panther.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Clipboard = System.Windows.Clipboard;
using ComboBox = System.Windows.Controls.ComboBox;
using DataFormats = System.Windows.DataFormats;
using DataObject = System.Windows.DataObject;
using Task = System.Threading.Tasks.Task;

namespace Panther.Windows
{
    public partial class CopyBinFilesWindow : Window
    {
        private readonly HashSet<string> _folderPathsSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public ObservableCollection<FileItem> FileItems { get; set; }

        public CopyBinFilesWindow()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            InitializeComponent();

            TextBoxSrcBin.Text = PantherHelper.GetBinDirPath();
            FileItems = new ObservableCollection<FileItem>();
            DataContext = this;
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ConfigureWindow();
            LoadDstPathList();
            LoadDstPathSelected();
            LoadProfiles();
            _ = InitDataAsync(GetSelectedItemsInProfile());
        }

        private async Task InitDataAsync(List<string> checkedItems)
        {
            await LoadFilesAndFoldersAsync(checkedItems);
            UpdateTotalFilesAndSize();
            UpdateButtonStates();
        }

        private void OnOpenSrcBin_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(TextBoxSrcBin.Text))
            {
                FileFolderHelper.OpenFolderInExplorer(TextBoxSrcBin.Text);
            }
        }

        private void OnBrowseDstBin_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderBrowser()
            {
                Folder = ComboBoxDstBin.Text
            };

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                ComboBoxDstBin.Text = dlg.Folder.TrimEnd('\\');
                SaveComboBoxDstBin();
            }
        }

        private void OnCopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            var selectedPaths = new List<string>();

            foreach (var item in FileItems)
            {
                if (item.IsChecked)
                {
                    selectedPaths.Add(item.Path);
                }
            }

            if (selectedPaths.Count > 0)
            {
                var data = new DataObject();
                data.SetData(DataFormats.FileDrop, selectedPaths.ToArray());

                Clipboard.SetDataObject(data, true);

                MessageBoxEx.Show(this, "Selected items are now on the clipboard.\nGo to the destination folder and press [Ctrl + V] to paste.",
                                  ButtonCopyToClipboard.Content.ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBoxEx.Show(this, "No items selected to copy.",
                                  ButtonCopyToClipboard.Content.ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void OnCopyToDst_Click(object sender, RoutedEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (!Directory.Exists(ComboBoxDstBin.Text))
            {
                var answer = MessageBoxEx.Show(this, "The destination path was not found.\nWould you like to try creating this folder?",
                                               ButtonCopyToDst.Content.ToString(), MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (answer == MessageBoxResult.No)
                {
                    return;
                }

                try
                {
                    Directory.CreateDirectory(ComboBoxDstBin.Text);
                }
                catch (Exception ex)
                {
                    MessageBoxEx.Show(this, $"An error occurred while creating the folder.\n{ex.Message}",
                                      "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            OutputPaneHelper.ActivatePantherOutputPane();
            OutputPaneHelper.ClearPantherOutputPane();

            List<FileItem> checkedItems = FileItems.Where(item => item.IsChecked).ToList();
            _ = new CopyProgressWindow(checkedItems, TextBoxSrcBin.Text, ComboBoxDstBin.Text) { Owner = this }.ShowDialog();
        }

        private void OnReset_Click(object sender, RoutedEventArgs e)
        {
            UpdateFileItemsFromCurrentProfile();
            UpdateTotalFilesAndSize();
            UpdateButtonStates();
        }

        private void OnUnselectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in FileItems)
            {
                item.IsChecked = false;
            }

            UpdateTotalFilesAndSize();
            UpdateButtonStates();
        }

        private void OnRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (ListViewFileFolder != null)
            {
                List<string> checkedItems = FileItems.Where(f => f.IsChecked == true).Select(f => f.Name).ToList();
                _ = InitDataAsync(checkedItems);
            }
        }

        private void OnComboBoxProfile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox.SelectedItem is ComboBoxItem)
            {
                UpdateFileItemsFromCurrentProfile();
                UpdateTotalFilesAndSize();
                UpdateButtonStates();
            }
        }

        private void OnComboBoxDstBin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateButtonStates();
        }

        private void OnComboBoxDstBin_TextChanged(object sender, RoutedEventArgs e)
        {
            UpdateButtonStates();
        }

        private void OnComboBoxDstBin_LostFocus(object sender, RoutedEventArgs e)
        {
            if (IsDstBinValid())
            {
                ComboBoxDstBin.Text = ComboBoxDstBin.Text.TrimEnd('\\');
                SaveComboBoxDstBin();
            }
        }

        private void OnCheckBoxSelectFile_Click(object sender, RoutedEventArgs e)
        {
            FileItem fileItem = (FileItem)((FrameworkElement)sender).DataContext;

            if (fileItem != null)
            {
                UpdateTotalFilesAndSize();
                UpdateButtonStates();
            }
        }

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private void ConfigureWindow()
        {
            const int GWL_STYLE = -16;
            const int WS_MINIMIZEBOX = 0x20000;

            var hwnd = new WindowInteropHelper(this).Handle;
            var style = GetWindowLong(hwnd, GWL_STYLE);
            style &= ~WS_MINIMIZEBOX;
            SetWindowLong(hwnd, GWL_STYLE, style);
        }

        private void SaveComboBoxDstBin()
        {
            if (_folderPathsSet.Add(ComboBoxDstBin.Text))
            {
                var MaxDstPath = 20;
                ComboBoxDstBin.Items.Insert(0, ComboBoxDstBin.Text);

                while (ComboBoxDstBin.Items.Count > MaxDstPath)
                {
                    var lastIndex = ComboBoxDstBin.Items.Count - 1;
                    var lastValue = ComboBoxDstBin.Items[lastIndex] as string;
                    ComboBoxDstBin.Items.RemoveAt(ComboBoxDstBin.Items.Count - 1);
                    _folderPathsSet.Remove(lastValue);
                }

                SaveDstPathList();
            }

            SaveDstPathSelected();
        }

        private void LoadDstPathList()
        {
            string[] paths = RegistryHelper.GetDstPathList();
            foreach (var path in paths)
            {
                if (_folderPathsSet.Add(path))
                {
                    ComboBoxDstBin.Items.Add(path);
                }
            }
        }

        private void SaveDstPathList()
        {
            RegistryHelper.SaveDstPathList(ComboBoxDstBin.Items.Cast<object>().Select(item => item.ToString()).ToArray());
        }

        private void LoadDstPathSelected()
        {
            string selected = RegistryHelper.GetDstPathSelected();
            if (!string.IsNullOrEmpty(selected) && _folderPathsSet.Contains(selected))
            {
                ComboBoxDstBin.Text = selected;
            }
            else if (ComboBoxDstBin.Items.Count > 0)
            {
                ComboBoxDstBin.SelectedIndex = 0;
            }
        }

        private void SaveDstPathSelected()
        {
            RegistryHelper.SaveDstPathSelected(ComboBoxDstBin.Text);
        }

        private void LoadProfiles()
        {
            ComboBoxProfile.Items.Clear();
            for (var type = Profile.Type.BasicExe_Configurations; type <= Profile.Type.AllExe_Configurations_Other; type++)
            {
                var name = Profile.GetProfileName(type);
                if (!string.IsNullOrEmpty(name))
                {
                    var newItem = new ComboBoxItem
                    {
                        Content = Profile.GetProfileName(type)
                    };

                    ComboBoxProfile.Items.Add(newItem);
                }
            }

            if (ComboBoxProfile.Items.Count > 0)
            {
                ComboBoxProfile.SelectedIndex = 0;
            }
        }

        private bool IsSrcBinValid()
        {
            return TextBoxSrcBin != null && !string.IsNullOrWhiteSpace(TextBoxSrcBin.Text) && Directory.Exists(TextBoxSrcBin.Text);
        }

        private bool IsDstBinValid()
        {
            return ComboBoxDstBin != null && TextBoxSrcBin != null && ComboBoxDstBin.Text != TextBoxSrcBin.Text &&
                !string.IsNullOrWhiteSpace(ComboBoxDstBin.Text) && Path.IsPathRooted(ComboBoxDstBin.Text);
        }

        private List<string> GetSelectedItemsInProfile()
        {
            switch ((Profile.Type)ComboBoxProfile.SelectedIndex)
            {
                case Profile.Type.BasicExe_Configurations:
                    return Profile.Files.BasicExe_Configurations;

                case Profile.Type.AllExe_Configurations:
                    return Profile.Files.AllExe_Configurations;

                case Profile.Type.AllExe_Configurations_Other:
                    return Profile.Files.AllExe_Configurations_Other;

                default:
                    return new List<string> { };
            }
        }

        private async Task LoadFilesAndFoldersAsync(List<string> checkedItems)
        {
            FileItems.Clear();

            if (IsSrcBinValid())
            {
                DirectoryInfo dirInfo = new DirectoryInfo(TextBoxSrcBin.Text);

                foreach (var dir in dirInfo.GetDirectories())
                {
                    var directorySize = await GetDirectorySizeAsync(dir);

                    FileItems.Add(new FileItem
                    {
                        Name = dir.Name,
                        Path = dir.FullName,
                        IsChecked = checkedItems.FindIndex(x => x.Equals(dir.Name, StringComparison.OrdinalIgnoreCase)) != -1,
                        IsFolder = true,
                        Icon = GetFileOrFolderIcon(dir.FullName),
                        Type = GetFileType(dir.FullName),
                        Count = Directory.GetFiles(dir.FullName, "*.*", SearchOption.AllDirectories).Length,
                        Size = directorySize
                    });
                }

                foreach (var file in dirInfo.GetFiles())
                {
                    FileItems.Add(new FileItem
                    {
                        Name = file.Name,
                        Path = file.FullName,
                        IsChecked = checkedItems.FindIndex(x => x.Equals(file.Name, StringComparison.OrdinalIgnoreCase)) != -1,
                        IsFolder = false,
                        Icon = GetFileOrFolderIcon(file.FullName),
                        Type = GetFileType(file.FullName),
                        Count = 1,
                        Size = file.Length
                    });
                }
            }
        }

        private async Task<long> GetDirectorySizeAsync(DirectoryInfo dirInfo)
        {
            return await Task.Run(() => GetDirectorySize(dirInfo));
        }

        private long GetDirectorySize(DirectoryInfo dirInfo)
        {
            long size = 0;

            try
            {
                FileInfo[] files = dirInfo.GetFiles();
                foreach (FileInfo file in files)
                {
                    size += file.Length;
                }

                DirectoryInfo[] dirs = dirInfo.GetDirectories();
                foreach (DirectoryInfo dir in dirs)
                {
                    size += GetDirectorySize(dir);
                }
            }
            catch (UnauthorizedAccessException)
            {
            }

            return size;
        }

        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(
            string pszPath,
            uint dwFileAttributes,
            ref SHFILEINFO psfi,
            uint cbFileInfo,
            uint uFlags);

        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        private string GetFileType(string path)
        {
            const uint SHGFI_TYPENAME = 0x000000400;
            SHFILEINFO shinfo = new SHFILEINFO();

            SHGetFileInfo(path, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), SHGFI_TYPENAME);
            return shinfo.szTypeName;
        }

        private BitmapImage GetFileOrFolderIcon(string path)
        {
            const uint SHGFI_ICON = 0x000000100;
            const uint SHGFI_SMALLICON = 0x000000001;
            SHFILEINFO shinfo = new SHFILEINFO();

            SHGetFileInfo(
                path,
                0,
                ref shinfo,
                (uint)Marshal.SizeOf(shinfo),
                SHGFI_ICON | SHGFI_SMALLICON);

            var icon = System.Drawing.Icon.FromHandle(shinfo.hIcon);
            return ConvertIconToBitmapImage(icon);
        }

        private BitmapImage ConvertIconToBitmapImage(Icon icon)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                icon.ToBitmap().Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Seek(0, SeekOrigin.Begin);
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = ms;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                return bitmap;
            }
        }

        private void UpdateFileItemsFromCurrentProfile()
        {
            var checkedItems = GetSelectedItemsInProfile();
            if (FileItems != null && FileItems.Any())
            {
                foreach (var item in FileItems)
                {
                    item.IsChecked = checkedItems.FindIndex(x => x.Equals(item.Name, StringComparison.OrdinalIgnoreCase)) != -1;
                }
            }
        }

        private void UpdateTotalFilesAndSize()
        {
            if (FileItems != null)
            {
                var selectedItems = FileItems.Where(c => c.IsChecked == true);

                if (TextBlockTotalFiles != null)
                {
                    TextBlockTotalFiles.Text = $"Total Files: {selectedItems.Sum(f => f.Count)}";
                }

                if (TextBlockTotalSize != null)
                {
                    TextBlockTotalSize.Text = $"Total Size: {selectedItems.Sum(f => f.Size) * 1.0 / (1024 * 1024):F2} MB";
                }
            }
        }

        private void UpdateButtonStates()
        {
            if (ButtonOpen != null)
            {
                ButtonOpen.IsEnabled = IsSrcBinValid();
            }

            if (ButtonCopyToDst != null && FileItems != null)
            {
                ButtonCopyToDst.IsEnabled = IsDstBinValid() && FileItems.Any(item => item.IsChecked);
            }

            if (ButtonCopyToClipboard != null && FileItems != null)
            {
                ButtonCopyToClipboard.IsEnabled = FileItems.Any(item => item.IsChecked);
            }
        }
    }
}
