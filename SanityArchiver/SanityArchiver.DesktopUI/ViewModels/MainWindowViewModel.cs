using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using SanityArchiver.DesktopUI.ViewModels.Commands;
using SanityArchiver.DesktopUI.Views;
using Directory = SanityArchiver.Application.Models.Directory;
using File = SanityArchiver.Application.Models.File;

namespace SanityArchiver.DesktopUI.ViewModels
{
    /// <summary>
    /// MainWindowViewModel
    /// </summary>
    public class MainWindowViewModel : DependencyObject, INotifyPropertyChanged
    {
        /// <summary>
        /// 
        /// </summary>
        public Directory Dir { get; set; } = new Directory();

        /// <summary>
        /// 
        /// </summary>
        public List<File> AllFiles { get; set; } = new List<File>();

        private const string Path = @"C:\Users\Kornél\codecool";

        /// <summary>
        /// 
        /// </summary>
        public List<File> FilesToCompress { get; set; } = new List<File>();

        /// <summary>
        /// 
        /// </summary>
        public List<File> FilesToEncrypt = new List<File>();

        /// <summary>
        /// 
        /// </summary>
        public List<File> FilesToDecrypt = new List<File>();

        /// <summary>
        /// 
        /// </summary>
        public File SelectedFile = new File();

        /// <summary>
        /// 
        /// </summary>
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
            dir.Path = path;

            foreach (var fileInFiles in files)
            {
                var fi = new FileInfo(fileInFiles);
                var file = new File(ref _openEnabled)
                {
                    FileName = System.IO.Path.GetFileName(fileInFiles),
                    DirectoryPath = System.IO.Path.GetDirectoryName(fileInFiles),
                    Size = fi.Length,
                    Created = fi.CreationTime,
                    IsChecked = false,
                    IsHidden = fi.Attributes.HasFlag(FileAttributes.Hidden),
                    Extension = System.IO.Path.GetExtension(fileInFiles)
                };

                dir.Files.Add(file);
                AllFiles.Add(file);
            }

            foreach (var directory in dirs)
            {
                var d = new Directory
                    {Name = directory.Substring(directory.LastIndexOf("\\", StringComparison.Ordinal) + 1)};
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


        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="files"></param>
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

        /// <summary>
        /// 
        /// </summary>
        public void ClearCheckingOnFiles()
        {
            foreach (var file in Files)
            {
                file.IsChecked = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filesToEncrypt"></param>
        /// <param name="extension"></param>
        public void ChangeFileExtension(IEnumerable<File> filesToEncrypt, string extension)
        {
            foreach (var file in filesToEncrypt)
            {
                System.IO.File.Move(file.FullPath,
                    System.IO.Path.ChangeExtension(file.FullPath, extension) ?? throw new InvalidOperationException());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newFileName"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string CutExtensionFromFileName(string fileName)
        {
            int fileExtPos = fileName.LastIndexOf(".", StringComparison.Ordinal);
            return fileExtPos >= 0 ? fileName.Substring(0, fileExtPos) : fileName;
        }

        private string _searchText;

        /// <summary>
        /// 
        /// </summary>
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged("SearchText");
                    SearchEnabled = _searchText.Length >= 3;
                }
            }
        }

        private string _foundFilesText;

        /// <summary>
        /// 
        /// </summary>
        public string FoundFilesText
        {
            get { return _foundFilesText; }
            set
            {
                if (_foundFilesText != value)
                {
                    _foundFilesText = value;
                    OnPropertyChanged("FoundFilesText");
                }
            }
        }

        private bool _searchEnabled;

        /// <summary>
        /// 
        /// </summary>
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

        private bool _openEnabled;

        /// <summary>
        /// 
        /// </summary>
        public bool OpenEnabled
        {
            get => _openEnabled;
            set
            {
                if (_openEnabled != value)
                {
                    _openEnabled = value;
                    OnPropertyChanged("OpenEnabled");
                }
            }
        }

        private string _directorySize;

        /// <summary>
        /// 
        /// </summary>
        public string DirectorySize
        {
            get { return _directorySize; }
            set
            {
                if (_directorySize != value)
                {
                    _directorySize = value;
                    OnPropertyChanged("DirectorySize");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Directory Directory { get; set; }

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<File> SearchFile()
        {
            ObservableCollection<File> foundFiles = new ObservableCollection<File>();
            foreach (var file in Files)
            {
                if (Regex.IsMatch(file.FileName, SearchText))
                {
                    foundFiles.Add(file);
                }
            }

            FoundFilesText = $"We found {foundFiles.Count} files matching the criteria.";

            OnPropertyChanged("FoundFilesText");
            return foundFiles;
        }
    }
}