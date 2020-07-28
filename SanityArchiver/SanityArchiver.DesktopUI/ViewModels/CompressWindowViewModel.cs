using System.ComponentModel;

namespace SanityArchiver.DesktopUI.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class CompressWindowViewModel : INotifyPropertyChanged
    {
        private string _fileName;

        /// <summary>
        /// 
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
            set
            {
                if (_fileName!= value)
                {
                    _fileName = value;
                    OnPropertyChanged("FileName");
                    SaveBtnEnabled = _fileName.Length >= 3;
                }
            }
        }

        private bool _saveBtnEnabled;

        /// <summary>
        /// 
        /// </summary>
        public bool SaveBtnEnabled
        {
            get { return _saveBtnEnabled; }
            set
            {
                if (_saveBtnEnabled != value)
                {
                    _saveBtnEnabled = value;
                    OnPropertyChanged("SaveBtnEnabled");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
