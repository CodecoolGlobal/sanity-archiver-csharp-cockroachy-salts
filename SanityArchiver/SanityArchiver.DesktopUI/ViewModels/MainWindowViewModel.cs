using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SanityArchiver.DesktopUI.Views;
using Directory = SanityArchiver.DesktopUI.Models.Directory;
using File = SanityArchiver.DesktopUI.Models.File;

namespace SanityArchiver.DesktopUI.ViewModels
{   /// <summary>
    /// MainWindowViewModel
    /// </summary>
    public class MainWindowViewModel : DependencyObject
    {
        public Directory _dir { get; set; } = new Directory();

        public List<File> AllFiles { get; set; } = new List<File>();

        private const string Path = "C:/Users/Tamás/Desktop/Projektek";

        public List<File> _filesToCompress { get; set; } = new List<File>();

        public List<File> _filesToEncrypt = new List<File>();

        public  List<File> _filesToDecrypt = new List<File>();

        public File _selectedFile = new File();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// Sets a List of Directory as ObservableCollection
        /// </summary>
        /// <param name="dirs"> List of directories</param>
        public MainWindowViewModel()
        {
            Seed();
            this.Directories = new ObservableCollection<Directory>(new List<Directory>(){_dir});
            this.Files = new ObservableCollection<File>(AllFiles);
        }

        private void Seed()
        {
            _directory = _dir;
            _directory = new Directory() { Name = "Projektek" };
            RecurseDir(Path, ref _directory);
            _dir = _directory;
        }

        private void RecurseDir(string path, ref Directory dir)
        {
            var files = System.IO.Directory.GetFiles(path);
            var dirs = System.IO.Directory.GetDirectories(path);

            dir.Name = path.Substring(path.LastIndexOf("\\", StringComparison.Ordinal) + 1);

            foreach (var fileInFiles in files)
            {
                var fi = new FileInfo(fileInFiles);

                var file = new File()
                {
                    FileName = System.IO.Path.GetFileName(fileInFiles),
                    DirectoryPath = System.IO.Path.GetDirectoryName(fileInFiles),
                    Size = fi.Length,
                    Created = fi.CreationTime,
                    IsChecked = false,
                    IsHidden = fi.Attributes.HasFlag(FileAttributes.Hidden),
                    Extension = System.IO.Path.GetExtension(fileInFiles),
                };

                dir.Files.Add(file);
                AllFiles.Add(file);
            }

            foreach (var directory in dirs)
            {
                var d = new Directory() { Name = directory.Substring(directory.LastIndexOf("\\", StringComparison.Ordinal) + 1) };
                RecurseDir(directory, ref d);
                dir.Directories.Add(d);
            }

        }

        /// <summary>
        /// It's a collection of Directories
        /// </summary>
        public ObservableCollection<Directory> Directories
        {
            get => (ObservableCollection<Directory>)GetValue(DirectoriesProperty);
            set => SetValue(DirectoriesProperty, value);
        }
        /// <summary>
        /// It's a collection of Files
        /// </summary>
        public ObservableCollection<File> Files
        {
            get => (ObservableCollection<File>)GetValue(FilesProperty);
            set => SetValue(FilesProperty, value);
        }

        /// <summary>
        /// Registers Directories as DependencyProperty
        /// </summary>
        public static readonly DependencyProperty DirectoriesProperty =
            DependencyProperty.Register("Directories", typeof(ObservableCollection<Directory>), typeof(Views.MainWindow), new UIPropertyMetadata(null));

        /// <summary>
        /// Registers Files as DependencyProperty
        /// </summary>
        public static readonly DependencyProperty FilesProperty =
            DependencyProperty.Register("Files", typeof(ObservableCollection<File>), typeof(Views.MainWindow), new UIPropertyMetadata(null));

        private Directory _directory;
    }
}
