using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SanityArchiver.DesktopUI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        private MainWindowViewModel _vm;

        private const string Path = "C:/Users/Tamás/Desktop/Projektek";

        private readonly List<File> _allFiles = new List<File>();

        private List<File> _filesToCompress = new List<File>();

        private List<File> _filesToEncrypt = new List<File>();

        private readonly List<File> _filesToDecrypt = new List<File>();

        private File _selectedFile;

        private Directory _dir = new Directory();

        /// <summary>
        /// We need this for the Folder Tree
        /// </summary>
        public MainWindowViewModel Vm
        {
            set
            {
                _vm = value;
                this.DataContext = _vm;
            }
        }
        /// <summary>
        /// When the Main Window is loaded it loads up "VM" with a List of Directories from the path(uses the recurseDir func)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _dir = new Directory() { Name = "Projektek" };
            RecurseDir(Path, ref _dir);

            Vm = new MainWindowViewModel(new List<Directory>() { _dir });
        }

        /// <summary>
        /// Recursively look for folders, subfolders, and files
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dir"></param>
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
                _allFiles.Add(file);
            }

            foreach (var directory in dirs)
            {
                var d = new Directory() { Name = directory.Substring(directory.LastIndexOf("\\", StringComparison.Ordinal) + 1) };
                RecurseDir(directory, ref d);
                dir.Directories.Add(d);
            }

        }

        /// <summary>
        /// Controls the browsing trough the directory tree
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void NameCol_mousedown(object sender, MouseButtonEventArgs e)
        {
            var tb = (TextBlock)e.OriginalSource;
            var dataObject = tb.DataContext;
            var dataSource = (Directory)dataObject;

            ShowFilesInGrid(dataSource.Files);

        }
        /// <summary>
        /// Refreshes MainWindow when any file manipulation has been made.
        /// </summary>
        public void RefreshBrowser()
        {
            new MainWindow();
        }

        /// <summary>
        /// Fills the DataGrid with the files from the selected Directory
        /// </summary>
        /// <param name="files"></param>

        private void ShowFilesInGrid(IEnumerable<File> files)
        {
            FilesDataGrid.ItemsSource = files;
        }

        private void CompressButton_Click(object sender, RoutedEventArgs e)
        {
            
            foreach (var file in _allFiles)
            {
                if (file.IsChecked)
                {
                    _filesToCompress.Add(file);
                }
            }
            OpenCompressWindows();
        }

        private void OpenCompressWindows()
        {
            CompressPopUp.Visibility = Visibility.Visible;
            
        }

        private void CompressTheFiles(IReadOnlyList<File> files)
        {
            using (var zip = ZipFile.Open(CompressName.Text+".zip", ZipArchiveMode.Create))
            {
                foreach (var file in files)
                {
                    zip.CreateEntryFromFile(file.FullPath, file.FileName);
                }

                Close();
            }

            string sourceLocation = "C:/Users/Tamás/source/repos/sanity-archiver-csharp-cockroachy-salts/SanityArchiver/SanityArchiver.DesktopUI/bin/Debug" + "/" + CompressName.Text + ".zip";
            string targetLocation = files[0].DirectoryPath + "/" + CompressName.Text + ".zip";

            System.IO.File.Move(sourceLocation, targetLocation);

            CompressPopUp.Visibility = Visibility.Hidden;
            

        }
        
        private void ZipButton_Click(object sender, RoutedEventArgs e)
        {
            CompressTheFiles(_filesToCompress);
            RefreshBrowser();
            CompressPopUp.Visibility = Visibility.Hidden;
            _filesToCompress = new List<File>();
            ClearCheckingOnFiles();
        }

        private void CompressCloseButton_Click(object sender, RoutedEventArgs e)
        {
            CompressPopUp.Visibility = Visibility.Hidden;
        }

        private void Encrypt(object sender, RoutedEventArgs e)
        {
            foreach (var file in _allFiles)
            {
                if (file.IsChecked)
                {
                    if (file.Extension == ".txt")
                    {
                        _filesToEncrypt.Add(file);
                    }
                    else
                    {
                        Console.WriteLine(@"Not a txt file");
                    }
                }
            }
            EncryptFiles();
            ClearCheckingOnFiles();
        }

        private void EncryptFiles()
        {
            foreach (var file in _filesToEncrypt)
            {
                System.IO.File.Encrypt(file.FullPath);
                ChangeFileExtension(_filesToEncrypt, ".ENC");
                _filesToEncrypt = new List<File>();
            }
            ClearCheckingOnFiles();
        }

        private void DecryptFiles(List<File> files)
        {
            foreach (var file in files)
            {
                try
                {
                    System.IO.File.Decrypt(file.FullPath);
                    ChangeFileExtension(files, ".txt");
                } catch (FileNotFoundException)
                {
                }
            }
            ClearCheckingOnFiles();
        }

        private void ClearCheckingOnFiles()
        {
            foreach (var file in _allFiles)
            {
                file.IsChecked = false;
            }
        }

        private void ChangeFileExtension(IEnumerable<File> filesToEncrypt, string extension)
        {
            foreach (var file in filesToEncrypt)
            {
                System.IO.File.Move(file.FullPath, System.IO.Path.ChangeExtension(file.FullPath, extension) ?? throw new InvalidOperationException());
            }
        }

        private void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var file in _allFiles)
            {
                if (file.IsChecked)
                {
                    if (file.Extension == ".ENC")
                    {
                        _filesToDecrypt.Add(file);
                    }
                }
            }

            DecryptFiles(_filesToDecrypt);

        }


        private void ChangeFileAttributesWindow()
        {
            AttribPopUp.Visibility = Visibility.Visible;
            AttribFileName.Text = CutExtensionFromFileName(_selectedFile.FileName);
            AttribExtension.Text = _selectedFile.Extension;
            AttribHidden.IsChecked = _selectedFile.IsHidden;
        }

        private string CutExtensionFromFileName(string fileName)
        {
            int fileExtPos = fileName.LastIndexOf(".", StringComparison.Ordinal);
            return fileExtPos >= 0 ? fileName.Substring(0, fileExtPos) : fileName;
        }

        private void AttribSaveButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedFile.Extension = AttribExtension.Text;
            if (AttribHidden.IsChecked != null)
            {
                _selectedFile.IsHidden = (bool) AttribHidden.IsChecked;
            }

            AttribPopUp.Visibility = Visibility.Hidden;
            SaveChangedFileData(AttribFileName.Text);
        }

        /// <summary>
        /// Saves all the new attributes from the Attribute changer window.
        /// </summary>
        /// <param name="newFileName">Provide the new file name for the file.</param>
        public void SaveChangedFileData(string newFileName)
        {
            if (_selectedFile.IsHidden)
            {
                System.IO.File.SetAttributes(_selectedFile.FullPath, System.IO.File.GetAttributes(_selectedFile.FullPath) | FileAttributes.Hidden);
            }
            else
            {
                System.IO.File.SetAttributes(_selectedFile.FullPath, FileAttributes.Normal);
            }
            System.IO.File.Move(_selectedFile.FullPath, System.IO.Path.ChangeExtension(_selectedFile.DirectoryPath + "/" + newFileName, _selectedFile.Extension));
        }

        private void AttribCloseButton_OnClickCloseButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedFile = new File();
            AttribPopUp.Visibility = Visibility.Hidden;
        }

        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        {
            _selectedFile = FilesDataGrid.SelectedItem as File;
            ChangeFileAttributesWindow();
        }

    }

    /// <summary>
    /// Interaction Logic for ViewModel
    /// </summary>
    public class MainWindowViewModel : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// Sets a List of Directory as ObservableCollection
        /// </summary>
        /// <param name="dirs"> List of directories</param>
        public MainWindowViewModel(List<Directory> dirs)
        {
            this.Directories = new ObservableCollection<Directory>(dirs);
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
        /// Registers Directories as DependencyProperty
        /// </summary>
        public static readonly DependencyProperty DirectoriesProperty =
            DependencyProperty.Register("Directories", typeof(ObservableCollection<Directory>), typeof(MainWindowViewModel), new UIPropertyMetadata(null));
    }
    /// <summary>
    /// Directory class
    /// </summary>
    public class Directory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Directory"/> class.
        /// Directory constructor
        /// </summary>
        public Directory()
        {
            Files = new List<File>();
            Directories = new List<Directory>();
        }
        /// <summary>
        /// Getter and setter for List of files
        /// </summary>
        public List<File> Files { get; private set; }
        /// <summary>
        /// Getter and setter for list of directories
        /// </summary>
        public List<Directory> Directories { get; private set; }
        /// <summary>
        /// Getter and setter for dir name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// File counter
        /// </summary>
        public int FileCount => Files.Count;

        /// <summary>
        /// Dir counter
        /// </summary>
        public int DirectoryCount => Directories.Count;

        /// <summary>
        /// To string method for dir names
        /// </summary>
        /// <returns> String of name</returns>
        public override string ToString()
        {
            return Name;
        }
    }


    /// <summary>
    /// File class
    /// </summary>
    public class File
    {
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
        public bool IsChecked { get; set; }
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
    }
}
