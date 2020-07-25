using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SanityArchiver.DesktopUI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        private MainWindowViewModel _vm;

        private static string path = "C:/Users/Tamás/Desktop/Projektek";

        private List<File> AllFiles = new List<File>();

        private List<File> FilesToCompress = new List<File>();

        private List<File> FilesToEncrypt = new List<File>();

        private List<File> FilesToDecrypt = new List<File>();

        private Directory dir = new Directory();

        public MainWindowViewModel VM
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
            dir = new Directory() { Name = "Projektek" };
            recurseDir(path, ref dir);

            VM = new MainWindowViewModel(new List<Directory>() { dir });
        }

        /// <summary>
        /// Recursively look for folders, subfolders, and files
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dir"></param>
        private void recurseDir(string path, ref Directory dir)
        {
            var files = System.IO.Directory.GetFiles(path);
            var dirs = System.IO.Directory.GetDirectories(path);

            dir.Name = path.Substring(path.LastIndexOf("\\", StringComparison.Ordinal) + 1);

            for (int i = 0; i < files.Length; i++)
            {
                
                var fi = new FileInfo(files[i]);

                var file = new File()
                {
                    FileName = System.IO.Path.GetFileName(files[i]),
                    DirectoryPath = System.IO.Path.GetDirectoryName(files[i]),
                    Size = fi.Length,
                    Created = fi.CreationTime,
                    IsChecked = false,
                    Extension = System.IO.Path.GetExtension(files[i]),
                };

                dir.Files.Add(file);
                AllFiles.Add(file);

            }

            for (int i = 0; i < dirs.Length; i++)
            {
                var d = new Directory() { Name = dirs[i].Substring(dirs[i].LastIndexOf("\\") + 1) };
                recurseDir(dirs[i], ref d);
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
            var dataCxtx = tb.DataContext;
            var dataSource = (Directory)dataCxtx;

            ShowFilesInGrid(dataSource.Files);

        }
        /// <summary>
        /// Refreshes MainWindow when any file manipulation has been made.
        /// </summary>
        public void RefreshBrowser()
        {
            MainWindow NewMainWindow = new MainWindow();
        }

        /// <summary>
        /// Fills the DataGrid with the files from the selected Directory
        /// </summary>
        /// <param name="Files"></param>

        private void ShowFilesInGrid(List<File> Files)
        {
            FilesDataGrid.ItemsSource = Files;
        }

        private void CompressButton_Click(object sender, RoutedEventArgs e)
        {
            
            foreach (var file in AllFiles)
            {
                if (file.IsChecked)
                {
                    FilesToCompress.Add(file);
                }
            }
            OpenCompressWindows();
        }

        private void OpenCompressWindows()
        {
            CompressPopUp.Visibility = Visibility.Visible;
            
        }

        private void CompressTheFiles(List<File> files)
        {
            using (ZipArchive zip = ZipFile.Open(CompressName.Text+".zip", ZipArchiveMode.Create))
            {
                foreach (var file in files)
                {
                    zip.CreateEntryFromFile(file.FullPath, file.FileName);
                }

                Close();
            }

            string SourceLocation = "C:/Users/Tamás/source/repos/sanity-archiver-csharp-cockroachy-salts/SanityArchiver/SanityArchiver.DesktopUI/bin/Debug" + "/" + CompressName.Text + ".zip";
            string TargetLocation = files[0].DirectoryPath + "/" + CompressName.Text + ".zip";

            System.IO.File.Move(SourceLocation, TargetLocation);

            CompressPopUp.Visibility = Visibility.Hidden;
            

        }
        
        private void ZipButton_Click(object sender, RoutedEventArgs e)
        {
            CompressTheFiles(FilesToCompress);
            RefreshBrowser();
            CompressPopUp.Visibility = Visibility.Hidden;
            FilesToCompress = new List<File>();
            ClearCheckingOnFiles();
        }

        private void CompressCloseButton_Click(object sender, RoutedEventArgs e)
        {
            CompressPopUp.Visibility = Visibility.Hidden;
        }

        private void Encrypt(object sender, RoutedEventArgs e)
        {
            foreach (var file in AllFiles)
            {
                if (file.IsChecked)
                {
                    if (file.Extension == ".txt")
                    {
                        FilesToEncrypt.Add(file);
                    }
                    else
                    {
                        Console.WriteLine("Not a txt file");
                    }
                }
            }
            EncryptFiles();
            ClearCheckingOnFiles();
        }

        private void EncryptFiles()
        {
            foreach (var file in FilesToEncrypt)
            {
                System.IO.File.Encrypt(file.FullPath);
                ChangeFileExtension(FilesToEncrypt, ".ENC",file.FileName);
                FilesToEncrypt = new List<File>();
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
                    ChangeFileExtension(files, ".txt", file.FileName + "_" + DateTime.Now.ToString());
                } catch (FileNotFoundException)
                {
                    continue;
                }
            }
            ClearCheckingOnFiles();
        }

        private void ClearCheckingOnFiles()
        {
            foreach (var file in AllFiles)
            {
                file.IsChecked = false;
            }
        }

        private void ChangeFileExtension(List<File> filesToEncrypt, string Extension, string FileName)
        {
            foreach (var file in filesToEncrypt)
            {
                System.IO.File.Move(file.FullPath, Path.ChangeExtension(file.FullPath, Extension));
            }
        }

        private void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var file in AllFiles)
            {
                if (file.IsChecked)
                {
                    if (file.Extension == ".ENC")
                    {
                        FilesToDecrypt.Add(file);
                    }
                }
            }

            DecryptFiles(FilesToDecrypt);

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
        /// <param name="Dirs"> List of directories</param>
        public MainWindowViewModel(List<Directory> Dirs)
        {
            this.Directories = new ObservableCollection<Directory>(Dirs);
        }

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

        public Directory BaseDir
        {
            get => (Directory)GetValue(BaseDirProperty);
            set => SetValue(BaseDirProperty, value);
        }

        public static readonly DependencyProperty BaseDirProperty =
            DependencyProperty.Register("BaseDir", typeof(Directory), typeof(MainWindowViewModel), new UIPropertyMetadata(null));


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
        public string DirectoryPath { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public double Size { get; set; }

        public bool IsChecked { get; set; }

        public DateTime Created { get; set; }
        public string FullPath => System.IO.Path.Combine(DirectoryPath, FileName);

        public override string ToString()
        {
            return FileName;
        }
    }
}
