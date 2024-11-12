using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace Panther.Windows
{
    public class FileItem : INotifyPropertyChanged
    {
        private bool _isChecked;

        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsFolder { get; set; }
        public string Type { get; set; }
        public int Count { get; set; }
        public long Size { get; set; }
        public BitmapImage Icon { get; set; }
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    OnPropertyChanged("IsChecked");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
