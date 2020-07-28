using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using SanityArchiver.DesktopUI.Annotations;

namespace SanityArchiver.DesktopUI.Models
{
    /// <summary>
    /// File class
    /// </summary>
    public class File : INotifyPropertyChanged
    {
        public File(ref bool openable)
        {
            Openable = openable;
        }

        public File(){}
        /// <summary>
        /// Path of the directory which holds the file
        /// </summary>
        public string DirectoryPath { get; set; }
        /// <summary>
        /// Name of the file
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// The extension of the file as string
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// Size of the file as double
        /// </summary>
        public double Size { get; set; }

        /// <summary>
        /// Property for the checkboxes in the DataGrid
        /// </summary>
        private bool _isChecked;

        public bool IsChecked
        {
            get { return _isChecked;}
            set
                {
                    if (_isChecked != value)
                    {
                        _isChecked = value;
                        OnPropertyChanged("IsChecked");
                    }
                }
            
        }

        public bool Openable;
        /// <summary>
        /// True if the file has the hidden attribute
        /// </summary>
        public bool IsHidden { get; set; }

        /// <summary>
        /// Date of creation in DateTime
        /// </summary>
        public DateTime Created { get; set; }
        /// <summary>
        /// Full path built from the Directory path + Filename
        /// </summary>
        public string FullPath => Path.Combine(DirectoryPath, FileName);

        /// <summary>
        /// ToString method which only returns the FileName
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return FileName;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.Extension == ".txt") Openable = true;
            Console.WriteLine(Openable);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
