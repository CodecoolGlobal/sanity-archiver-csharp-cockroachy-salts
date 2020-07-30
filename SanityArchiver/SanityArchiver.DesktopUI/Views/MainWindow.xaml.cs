using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SanityArchiver.DesktopUI.ViewModels;

namespace SanityArchiver.DesktopUI.Views
{
    public partial class MainWindow : Window
    {
        private DataGrid DataGrid;
        private List<DirectoryItemViewModel> Files = new List<DirectoryItemViewModel>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new DirectoryStructureViewModel();
        }

        private void FolderView_MouseDown(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            FileView.ItemsSource =
                DirectoryStructure.GetDirectoryFiles(((DirectoryItemViewModel)FolderView.SelectedItem).FullPath);
            try
            {
                DirectoryItemViewModel directory = (DirectoryItemViewModel)FolderView.SelectedItem;
                long byteSize = DirSize(new DirectoryInfo(directory.FullPath));
                DirectorySize.Content = $"The size of the folder is: {FileSizeFormatter.FormatSize(byteSize)}";
            }
            catch (Exception)
            {
                DirectorySize.Content = "Can't evaluate folder's size.";
            }
        }

        private void CompressButton_Click(object sender, RoutedEventArgs e)
        {
            List<DirectoryItemViewModel> files =
                DirectoryStructure.GetCheckedFiles((List<DirectoryItemViewModel>)FileView.ItemsSource);
            if (files == null || files.Count == 0)
            {
                return;
            }
            CompressWindow compressWindow = new CompressWindow(files);
            compressWindow.Closing += Refresh;
            compressWindow.ShowDialog();
        }

        private void EncryptButton_Click(object sender, RoutedEventArgs e)
        {
            DirectoryItemViewModel file = (DirectoryItemViewModel)DataGrid.SelectedItem;
            if (file == null)
            {
                return;
            }
            EncryptWindow encryptWindow = new EncryptWindow(file);
            encryptWindow.Closing += Refresh;
            encryptWindow.ShowDialog();
        }

        private void Refresh(object sender, CancelEventArgs e)
        {
            FolderView_MouseDown(null, null);
        }

        private void FileView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DirectoryItemViewModel file = (DirectoryItemViewModel)DataGrid.SelectedItem;
            if (file == null)
            {
                return;
            }
            OpenButton.IsEnabled = file.Extension == ".txt";
            EncryptButton.IsEnabled = file.Extension == ".txt";
            DecryptButton.IsEnabled = file.Extension == ".ENC";
        }

        private void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            DirectoryItemViewModel file = (DirectoryItemViewModel)DataGrid.SelectedItem;
            File.Decrypt(file.FullPath);
            string newPath = Path.ChangeExtension(file.FullPath, ".txt");
            File.Move(file.FullPath,
                newPath);
            File.Move(newPath, $"{newPath.Substring(0, newPath.Length - file.Extension.Length)}_" +
                               $"{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}_{DateTime.Now.Hour}_{DateTime.Now.Minute}.txt");
            FolderView_MouseDown(null, null);
        }

        private void DataGridRow_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DirectoryItemViewModel file = (DirectoryItemViewModel)DataGrid.SelectedItem;
            FileAttributeWindow fileAttributeWindow = new FileAttributeWindow(file);
            fileAttributeWindow.Closing += Refresh;
            fileAttributeWindow.ShowDialog();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            DirectoryItemViewModel file = (DirectoryItemViewModel)DataGrid.SelectedItem;
            TxtWindow txtWindow = new TxtWindow();
            txtWindow.TextBlock.Text = File.ReadAllText(file.FullPath);
            txtWindow.ShowDialog();
        }

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

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            List<DirectoryItemViewModel> files =
                DirectoryStructure.GetCheckedFiles((List<DirectoryItemViewModel>)FileView.ItemsSource);
            if (files == null || files.Count == 0)
            {
                return;
            }
            Files.Clear();
            Files.AddRange(files);
            CopyButton.IsEnabled = false;
            MoveButton.IsEnabled = false;
            PasteButton.IsEnabled = true;
            PasteButton.Tag = "Copy";
        }

        private void MoveButton_Click(object sender, RoutedEventArgs e)
        {
            List<DirectoryItemViewModel> files =
                DirectoryStructure.GetCheckedFiles((List<DirectoryItemViewModel>)FileView.ItemsSource);
            if (files == null || files.Count == 0)
            {
                return;
            }
            Files.Clear();
            Files.AddRange(files);
            CopyButton.IsEnabled = false;
            MoveButton.IsEnabled = false;
            PasteButton.IsEnabled = true;
            PasteButton.Tag = "Move";
        }

        private void PasteButton_Click(object sender, RoutedEventArgs e)
        {
            DirectoryItemViewModel destinationDirectory = (DirectoryItemViewModel)FolderView.SelectedItem;
            foreach (var file in Files)
            {
                try
                {
                    File.Copy(file.FullPath, $"{destinationDirectory.FullPath}\\{file.Name}");
                }
                catch (Exception)
                {
                }
            }

            if (PasteButton.Tag == "Move")
            {
                foreach (var file in Files)
                {
                    File.Delete(file.FullPath);
                }
            }

            Files.Clear();
            CopyButton.IsEnabled = true;
            MoveButton.IsEnabled = true;
            PasteButton.IsEnabled = false;
            FolderView_MouseDown(null, null);
        }

        private IEnumerable<DirectoryItemViewModel> SearchAccessibleFiles(string root, string searchTerm)
        {
            var files = new List<DirectoryItemViewModel>();

            foreach (var file in System.IO.Directory.EnumerateFiles(root).Where(m => Regex.IsMatch(m, searchTerm)))
            {
                files.Add(new DirectoryItemViewModel(file, DirectoryItemType.File));
            }

            foreach (var subDir in System.IO.Directory.EnumerateDirectories(root))
            {
                try
                {
                    files.AddRange(SearchAccessibleFiles(subDir, searchTerm));
                }
                catch (UnauthorizedAccessException)
                {
                }
            }

            return files;
        }

        private void SearchButton_OnClickButton_Click(object sender, RoutedEventArgs e)
        {
            DirectoryItemViewModel directory = (DirectoryItemViewModel)FolderView.SelectedItem;
            IEnumerable<DirectoryItemViewModel> foundFiles = SearchAccessibleFiles(directory.FullPath, SearchText.Text);
            var directoryItemViewModels = foundFiles as DirectoryItemViewModel[] ?? foundFiles.ToArray();
            DirectorySize.Content = $"We found {directoryItemViewModels.Count()} files.";
            DataGrid.ItemsSource = directoryItemViewModels;
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox searchText = (TextBox)sender;
            SearchButton.IsEnabled = searchText.Text.Length >= 3;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataGrid = FileView;
        }
    }
}