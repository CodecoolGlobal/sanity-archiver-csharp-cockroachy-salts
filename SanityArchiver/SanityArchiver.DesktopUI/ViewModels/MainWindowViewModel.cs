using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using SanityArchiver.DesktopUI.ViewModels.Commands;
using SanityArchiver.DesktopUI.Views;
using Directory = SanityArchiver.DesktopUI.Models.Directory;
using File = SanityArchiver.DesktopUI.Models.File;

namespace SanityArchiver.DesktopUI.ViewModels
{
    /// <summary>
    /// MainWindowViewModel
    /// </summary>
    public class MainWindowViewModel : DependencyObject, INotifyPropertyChanged
    {
        public Directory Dir { get; set; } = new Directory();

        public List<File> AllFiles { get; set; } = new List<File>();

        private const string Path = "C:/Users/Kornél/codecool";

        public List<File> FilesToCompress { get; set; } = new List<File>();

        public List<File> FilesToEncrypt = new List<File>();

        public List<File> FilesToDecrypt = new List<File>();

        public File SelectedFile = new File();

        public SearchCommand SearchCommand { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// Sets a List of Directory as ObservableCollection
        /// </summary>
        public MainWindowViewModel()
        {
            Seed();
            Directories = new ObservableCollection<Directory>(new List<Directory> {Dir});
            Files = new ObservableCollection<File>(AllFiles);
            SearchCommand = new SearchCommand(this);
        }

        private void Seed()
        {
            _directory = Dir;
            _directory = new Directory {Name = "codecool"};
            RecurseDir(Path, ref _directory);
            Dir = _directory;
        }

        private void RecurseDir(string path, ref Directory dir)
        {
            var files = System.IO.Directory.GetFiles(path);
            var dirs = System.IO.Directory.GetDirectories(path);

            dir.Name = path.Substring(path.LastIndexOf("\\", StringComparison.Ordinal) + 1);

            foreach (var fileInFiles in files)
            {
                var fi = new FileInfo(fileInFiles);

                var file = new File
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
                var d = new Directory {Name = directory.Substring(directory.LastIndexOf("\\", StringComparison.Ordinal) + 1)};
                RecurseDir(directory, ref d);
                dir.Directories.Add(d);
            }

        }

        /// <summary>
        /// It's a collection of Directories
        /// </summary>
        public ObservableCollection<Directory> Directories
        {
            get => (ObservableCollection<Directory>) GetValue(DirectoriesProperty);
            set => SetValue(DirectoriesProperty, value);
        }

        /// <summary>
        /// It's a collection of Files
        /// </summary>
        public ObservableCollection<File> Files
        {
            get => (ObservableCollection<File>) GetValue(FilesProperty);
            set => SetValue(FilesProperty, value);
        }

        /// <summary>
        /// Registers Directories as DependencyProperty
        /// </summary>
        public static readonly DependencyProperty DirectoriesProperty =
            DependencyProperty.Register("Directories", typeof(ObservableCollection<Directory>),
                typeof(MainWindow), new UIPropertyMetadata(null));

        /// <summary>
        /// Registers Files as DependencyProperty
        /// </summary>
        public static readonly DependencyProperty FilesProperty =
            DependencyProperty.Register("Files", typeof(ObservableCollection<File>), typeof(MainWindow),
                new UIPropertyMetadata(null));

        private Directory _directory;


        public void EncryptFiles()
        {
            foreach (var file in FilesToEncrypt)
            {
                System.IO.File.Encrypt(file.FullPath);
                ChangeFileExtension(FilesToEncrypt, ".ENC");
                FilesToEncrypt = new List<File>();
            }

            ClearCheckingOnFiles();
        }

        public void DecryptFiles(List<File> files)
        {
            foreach (var file in files)
            {
                try
                {
                    System.IO.File.Decrypt(file.FullPath);
                    ChangeFileExtension(files, ".txt");
                }
                catch (FileNotFoundException)
                {
                }
            }

            ClearCheckingOnFiles();
        }

        public void ClearCheckingOnFiles()
        {
            foreach (var file in Files)
            {
                file.IsChecked = false;
            }
        }

        public void ChangeFileExtension(IEnumerable<File> filesToEncrypt, string extension)
        {
            foreach (var file in filesToEncrypt)
            {
                System.IO.File.Move(file.FullPath,
                    System.IO.Path.ChangeExtension(file.FullPath, extension) ?? throw new InvalidOperationException());
            }
        }

        public void SaveChangedFileData(string newFileName)
        {
            if (SelectedFile.IsHidden)
            {
                System.IO.File.SetAttributes(SelectedFile.FullPath,
                    System.IO.File.GetAttributes(SelectedFile.FullPath) | FileAttributes.Hidden);
            }
            else
            {
                System.IO.File.SetAttributes(SelectedFile.FullPath, FileAttributes.Normal);
            }

            System.IO.File.Move(SelectedFile.FullPath,
                System.IO.Path.ChangeExtension(SelectedFile.DirectoryPath + "/" + newFileName,
                    SelectedFile.Extension));
        }

        public string CutExtensionFromFileName(string fileName)
        {
            int fileExtPos = fileName.LastIndexOf(".", StringComparison.Ordinal);
            return fileExtPos >= 0 ? fileName.Substring(0, fileExtPos) : fileName;
        }

        private string _boundText;

        public string BoundText
        {
            get { return _boundText; }
            set
            {
                if (_boundText != value)
                {
                    _boundText = value;
                    OnPropertyChanged("BoundText");
                    SearchEnabled = _boundText.Length >= 3;
                }
            }
        }

        private string _foundFilesText = "TEST";

        public string FoundFilesText
        {
            get { return _foundFilesText; }
            set
            {
                if (_foundFilesText!= value)
                {
                    _foundFilesText = value;
                    OnPropertyChanged("FoundFilesText");
                }
            }
        }

        private bool _searchEnabled = false;

        public bool SearchEnabled
        {
            get { return _searchEnabled; }
            set
            {
                if (_searchEnabled != value)
                {
                    _searchEnabled = value;
                    OnPropertyChanged("SearchEnabled");
                }
            }
        }

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                Console.WriteLine(propertyName);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ObservableCollection<File> SearchFile()
        {
            ObservableCollection<File> FoundFiles = new ObservableCollection<File>();
            foreach (var file in Files)
            {
                if (Regex.IsMatch(file.FileName, BoundText))
                {
                    FoundFiles.Add(file);
                }
            }

            FoundFilesText = $"We found {FoundFiles.Count} files matching the criteria.";
            OnPropertyChanged("FoundFilesText");
            return FoundFiles;
        }
    }
}
