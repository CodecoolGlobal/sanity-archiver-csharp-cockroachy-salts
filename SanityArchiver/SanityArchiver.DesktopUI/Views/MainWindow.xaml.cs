﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SanityArchiver.DesktopUI.ViewModels;
using Directory = SanityArchiver.DesktopUI.Models.Directory;
using File = SanityArchiver.DesktopUI.Models.File;

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

        /// <summary>
        /// We need this for the Folder Tree
        /// </summary>
        public MainWindowViewModel Vm
        {
            get => this._vm;
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
            Vm = new MainWindowViewModel();
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
            var mainWindow = new MainWindow();
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
            foreach (var file in _vm.Files)
            {
                Console.WriteLine(file.FileName);
                if (file.IsChecked)
                {
                    Console.WriteLine("Found checked");
                    _vm._filesToCompress.Add(file);
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
            CompressTheFiles(_vm._filesToCompress);
            RefreshBrowser();
            CompressPopUp.Visibility = Visibility.Hidden;
            _vm._filesToCompress = new List<File>();
            _vm.ClearCheckingOnFiles();
        }

        private void CompressCloseButton_Click(object sender, RoutedEventArgs e)
        {
            CompressPopUp.Visibility = Visibility.Hidden;
        }

        private void Encrypt(object sender, RoutedEventArgs e)
        {
            foreach (var file in _vm.Files)
            {
                if (file.IsChecked)
                {
                    if (file.Extension == ".txt")
                    {
                        _vm._filesToEncrypt.Add(file);
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
                if (file.IsChecked)
                {
                    if (file.Extension == ".ENC")
                    {
                        _vm._filesToDecrypt.Add(file);
                    }
                }
            }

            _vm.DecryptFiles(_vm._filesToDecrypt);

        }


        private void ChangeFileAttributesWindow()
        {
            AttribPopUp.Visibility = Visibility.Visible;
            AttribFileName.Text = _vm.CutExtensionFromFileName(_vm._selectedFile.FileName);
            AttribExtension.Text = _vm._selectedFile.Extension;
            AttribHidden.IsChecked = _vm._selectedFile.IsHidden;
        }


        private void AttribSaveButton_Click(object sender, RoutedEventArgs e)
        {
            _vm._selectedFile.Extension = AttribExtension.Text;
            if (AttribHidden.IsChecked != null)
            {
                _vm._selectedFile.IsHidden = (bool) AttribHidden.IsChecked;
            }

            AttribPopUp.Visibility = Visibility.Hidden;
            _vm.SaveChangedFileData(AttribFileName.Text);
        }

        /// <summary>
        /// Saves all the new attributes from the Attribute changer window.
        /// </summary>
        /// <param name="newFileName">Provide the new file name for the file.</param>
        

        private void AttribCloseButton_OnClickCloseButton_Click(object sender, RoutedEventArgs e)
        {
            _vm._selectedFile = new File();
            AttribPopUp.Visibility = Visibility.Hidden;
        }

        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        {
            _vm._selectedFile = FilesDataGrid.SelectedItem as File;
            ChangeFileAttributesWindow();
        }

    }
}
