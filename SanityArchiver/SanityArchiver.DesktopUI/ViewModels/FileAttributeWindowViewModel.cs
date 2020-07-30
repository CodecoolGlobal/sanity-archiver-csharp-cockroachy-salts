using System.ComponentModel;
using SanityArchiver.ViewModels;

namespace SanityArchiver.DesktopUI.ViewModels
{
    public class FileAttributeWindowViewModel : BaseViewModel
    {
        private string _fileName;

        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                if (_fileName != value)
                {
                    _fileName = value;
                    OnPropertyChanged("FileName");
                    SaveBtnEnabled = _fileName.Length >= 3;
                    OnPropertyChanged("SaveBtnEnabled");
                }
            }
        }

        public string Extension { get; set; }

        public bool IsHidden { get; set; }

        public bool SaveBtnEnabled { get; set; }

        public new event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}