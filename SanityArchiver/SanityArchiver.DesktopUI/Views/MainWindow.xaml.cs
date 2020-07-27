using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SanityArchiver.DesktopUI.Models;
using SanityArchiver.DesktopUI.ViewModels;

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
        private void NameCol_mousedown(object sender, MouseButtonEventArgs e)
        {
            var tb = (TextBlock) e.OriginalSource;
            var dataObject = tb.DataContext;
            var dataSource = (Directory) dataObject;

            ShowFilesInGrid(dataSource.Files);
        }

        /// <summary>
        ///     Refreshes MainWindow when any file manipulation has been made.
        /// </summary>
        public void RefreshBrowser()
        {
            var mainWindow = new MainWindow();
        }

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
                Console.WriteLine(file.FileName);
                if (file.IsChecked)
                {
                    Console.WriteLine("Found checked");
                    _vm.FilesToCompress.Add(file);
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
            using (var zip = ZipFile.Open(CompressName.Text + ".zip", ZipArchiveMode.Create))
            {
                foreach (var file in files) zip.CreateEntryFromFile(file.FullPath, file.FileName);

                Close();
            }

            var sourceLocation =
                "C:/Users/Kornél/codecool/4_.NET/2_TW/sanity-archiver-csharp-cockroachy-salts/SanityArchiver/SanityArchiver.DesktopUI/bin/Debug" +
                "/" + CompressName.Text + ".zip";
            var targetLocation = files[0].DirectoryPath + "/" + CompressName.Text + ".zip";

            System.IO.File.Move(sourceLocation, targetLocation);

            CompressPopUp.Visibility = Visibility.Hidden;
        }

        private void ZipButton_Click(object sender, RoutedEventArgs e)
        {
            CompressTheFiles(_vm.FilesToCompress);
            RefreshBrowser();
            CompressPopUp.Visibility = Visibility.Hidden;
            _vm.FilesToCompress = new List<File>();
            _vm.ClearCheckingOnFiles();
        }

        private void CompressCloseButton_Click(object sender, RoutedEventArgs e)
        {
            CompressPopUp.Visibility = Visibility.Hidden;
        }

        private void Encrypt(object sender, RoutedEventArgs e)
        {
            foreach (var file in _vm.Files)
                if (file.IsChecked)
                    if (file.Extension == ".txt")
                        _vm.FilesToEncrypt.Add(file);
                    else
                        Console.WriteLine(@"Not a txt file");

            _vm.EncryptFiles();
            _vm.ClearCheckingOnFiles();
        }


        private void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var file in _vm.Files)
                if (file.IsChecked)
                    if (file.Extension == ".ENC")
                        _vm.FilesToDecrypt.Add(file);

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
            if (AttribHidden.IsChecked != null) _vm.SelectedFile.IsHidden = (bool) AttribHidden.IsChecked;

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
    }
}