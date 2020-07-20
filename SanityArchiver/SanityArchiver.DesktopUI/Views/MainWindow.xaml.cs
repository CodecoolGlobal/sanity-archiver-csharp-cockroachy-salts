using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using Microsoft.Win32;

namespace SanityArchiver.DesktopUI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private object buttonClicked;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        private MainWindowViewModel _vm;

        public MainWindowViewModel VM
        {
            set
            {
                _vm = value;
                this.DataContext = _vm;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var d = new Directory() {Name = "Projektek"};
            recurseDir("C:/Users/Tamás/Desktop/Projektek", ref d);

            VM = new MainWindowViewModel(new List<Directory>() {d});
        }

        private void recurseDir(string path, ref Directory dir)
        {
            var files = System.IO.Directory.GetFiles(path);
            var dirs = System.IO.Directory.GetDirectories(path);

            dir.Name = path.Substring(path.LastIndexOf("\\", StringComparison.Ordinal) + 1);

            for (int i = 0; i < files.Length; i++)
            {
                var fi = new FileInfo(files[i]);
                dir.Files.Add(new File()
                {
                    FileName = System.IO.Path.GetFileName(files[i]),
                    DirectoryPath = System.IO.Path.GetDirectoryName(files[i]),
                    Size = fi.Length,
                    Created = fi.CreationTime,
                    Extension = System.IO.Path.GetExtension(files[i])
                });

            }

            for (int i = 0; i < dirs.Length; i++)
            {
                var d = new Directory() {Name = dirs[i].Substring(dirs[i].LastIndexOf("\\") + 1)};
                recurseDir(dirs[i], ref d);
                dir.Directories.Add(d);

            }

        }

        private void NameCol_mousedown(object sender, MouseButtonEventArgs e)
        {
            var tb = (TextBlock) e.OriginalSource;
            var dataCxtx = tb.DataContext;
            var dataSource =(Directory) dataCxtx;

            ShowFilesInGrid(dataSource.Files);
            
        }

        private void ShowFilesInGrid(List<File> Files)
        {
            FilesDataGrid.ItemsSource = Files;
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
    /// Directory
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
    /// 
    /// </summary>
    public class File
    {
        public string DirectoryPath { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public double Size { get; set; }

        public DateTime Created { get; set; }
        public string FullPath => System.IO.Path.Combine(DirectoryPath, FileName);

        public override string ToString()
        {
            return FileName;
        }
    }
}
