using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace SanityArchiver.Application.Models
{
    /// <summary>
    /// File class
    /// </summary>
    public class File : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="File"/> class>
        /// </summary>
        /// <param name="openable"></param>
        public File(ref bool openable)
        {
            Openable = openable;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="File"/> class.
        /// </summary>
        public File()
        {
        }

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

        /// <summary>
        ///
        /// </summary>
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool Openable { get; set; }

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

        /// <summary>
        ///
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (Extension == ".txt")
            {
                Openable = true;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}