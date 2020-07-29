using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SanityArchiver.DesktopUI.ViewModels;
using Directory = SanityArchiver.Application.Models.Directory;
using File = SanityArchiver.Application.Models.File;

namespace SanityArchiver.DesktopUI.Views
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private MainWindowViewModel _vm;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     We need this for the Folder Tree
        /// </summary>
        public MainWindowViewModel Vm
        {
            get => _vm;
            set
            {
                _vm = value;
                DataContext = _vm;
            }
        }

        /// <summary>
        ///     When the Main Window is loaded it loads up "VM" with a List of Directories from the path(uses the recurseDir func)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Vm = new MainWindowViewModel();
        }


        /// <summary>
        ///     Controls the browsing trough the directory tree
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DirectoryTree_Mousedown(object sender, MouseButtonEventArgs e)
        {
            var tb = (TextBlock) e.OriginalSource;
            var dataObject = tb.DataContext;
            var dataSource = (Directory) dataObject;

            ShowFilesInGrid(dataSource.Files);
        }

        /// <summary>
        ///     Refreshes MainWindow when any file manipulation has been made.
        /// </summary>
        /// <summary>
        ///     Fills the DataGrid with the files from the selected Directory
        /// </summary>
        /// <param name="files"></param>
        private void ShowFilesInGrid(IEnumerable<File> files)
        {
            FilesDataGrid.ItemsSource = files;
        }

        private void CompressButton_Click(object sender, RoutedEventArgs e)
        {
            
            foreach (var file in _vm.Files)
            {
                if (file.IsChecked)
                {
                    Console.WriteLine(@"Found checked");
                    _vm.FilesToCompress.Add(file);
                }
            }

            if (_vm.FilesToCompress.Count < 1)
            {
                new ErrorWindow().Show();
                return;
            }
            CompressWindow compressWindow = new CompressWindow(_vm.FilesToCompress);
            compressWindow.Show();
            _vm.FilesToCompress = new List<File>();
        }

        private void Encrypt(object sender, RoutedEventArgs e)
        {
            foreach (var file in _vm.Files)
            {
                if (file.IsChecked)
                {
                    if (file.Extension == ".txt")
                    {
                        _vm.FilesToEncrypt.Add(file);
                    }
                    else
                    {
                        Console.WriteLine(@"Not a txt file");
                    }
                }
            }

            _vm.EncryptFiles();
            _vm.ClearCheckingOnFiles();
        }


        private void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var file in _vm.Files)
            {
                if (!file.IsChecked)
                {
                    continue;
                }

                if (file.Extension == ".ENC")
                {
                    _vm.FilesToDecrypt.Add(file);
                }
            }

            _vm.DecryptFiles(_vm.FilesToDecrypt);
        }


        private void ChangeFileAttributesWindow()
        {
            AttribPopUp.Visibility = Visibility.Visible;
            AttribFileName.Text = _vm.CutExtensionFromFileName(_vm.SelectedFile.FileName);
            AttribExtension.Text = _vm.SelectedFile.Extension;
            AttribHidden.IsChecked = _vm.SelectedFile.IsHidden;
        }


        private void AttribSaveButton_Click(object sender, RoutedEventArgs e)
        {
            _vm.SelectedFile.Extension = AttribExtension.Text;
            if (AttribHidden.IsChecked != null)
            {
                _vm.SelectedFile.IsHidden = (bool) AttribHidden.IsChecked;
            }

            AttribPopUp.Visibility = Visibility.Hidden;
            _vm.SaveChangedFileData(AttribFileName.Text);
        }

        /// <summary>
        ///     Saves all the new attributes from the Attribute changer window.
        /// </summary>
        private void AttribCloseButton_OnClickCloseButton_Click(object sender, RoutedEventArgs e)
        {
            _vm.SelectedFile = new File();
            AttribPopUp.Visibility = Visibility.Hidden;
        }

        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        {
            _vm.SelectedFile = FilesDataGrid.SelectedItem as File;
            ChangeFileAttributesWindow();
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var binding = ((TextBox) sender).GetBindingExpression(TextBox.TextProperty);
            binding?.UpdateSource();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ShowFilesInGrid(_vm.SearchFile());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            TxtWindow txtWindow = new TxtWindow();
            txtWindow.TextBlock.Text = System.IO.File.ReadAllText(_vm.SelectedFile.FullPath);
            txtWindow.Show();
        }

        private void FilesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid) sender;
            File file = (File) dataGrid.SelectedItem;
            if(file == null) return;
            foreach (var vmFile in _vm.Files)
            {
                if (vmFile.FullPath == file.FullPath)
                {
                    _vm.OpenEnabled = vmFile.Extension == ".txt";
                    _vm.SelectedFile = vmFile;
                    return;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static long DirSize(DirectoryInfo d)
        {
            long size = 0;
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }

            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += DirSize(di);
            }

            return size;
        }

        private void TreeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                Directory directory = (Directory) TreeView1.SelectedItem;
                long byteSize = DirSize(new DirectoryInfo(directory.Path));
                _vm.DirectorySize = FileSizeFormatter.FormatSize(byteSize);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class FileSizeFormatter
    {
        // Load all suffixes in an array  
        static readonly string[] Suffixes =
            {"Bytes", "KB", "MB", "GB", "TB", "PB"};

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string FormatSize(long bytes)
        {
            int counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }

            return string.Format("{0:n1}{1}", number, Suffixes[counter]);
        }
    }
}