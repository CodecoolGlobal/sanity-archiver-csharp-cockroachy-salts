using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanityArchiver.DesktopUI.ViewModels
{
    public class AttributeWindowViewModel : INotifyPropertyChanged
    {
        private string _fileName;
        private string _extension;
        private bool _isHidden;

        public string FileName
        {
            get { return _fileName; }
            set
            {
                if (_fileName != value)
                {
                    _fileName = value;
                    OnPropertyChanged("FileName");
                    SaveBtnEnabled = _fileName.Length >= 3;
                }
            }
        }

        public string Extension
        {
            get { return _extension; }
            set
            {
                if (_extension != value)
                {
                    _extension = value;
                    OnPropertyChanged("Extension");
                }
            }
        }

        public bool IsHidden
        {
            get { return _isHidden; }
            set
            {
                if (_isHidden != value)
                {
                    _isHidden = value;
                    OnPropertyChanged("IsHidden");
                }
            }
        }

        private bool _saveBtnEnabled;

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

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
