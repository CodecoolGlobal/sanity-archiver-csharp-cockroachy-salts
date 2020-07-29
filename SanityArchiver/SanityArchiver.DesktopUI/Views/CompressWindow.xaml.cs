using SanityArchiver.Application.Models;
using SanityArchiver.DesktopUI.ViewModels;
using System.Collections.Generic;
using System.IO.Compression;
using System.Windows;
using System.Windows.Controls;

namespace SanityArchiver.DesktopUI.Views
{
    /// <summary>
    /// Interaction logic for CompressWindow.xaml
    /// </summary>
    public partial class CompressWindow
    {
        private CompressWindowViewModel _vm;

        /// <summary>
        ///
        /// </summary>
        public CompressWindowViewModel Vm
        {
            get => _vm;
            set
            {
                _vm = value;
                DataContext = _vm;
            }
        }

        private List<File> _files;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompressWindow"/> class.
        /// </summary>
        /// <param name="files"></param>
        public CompressWindow(List<File> files)
        {
            _files = files;
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CompressTheFiles()
        {
            using (var zip = ZipFile.Open(FileName.Text + ".zip", ZipArchiveMode.Create))
            {
                foreach (var file in _files) zip.CreateEntryFromFile(file.FullPath, file.FileName);

                Close();
            }

            var sourceLocation =
                "C:\\Users\\Kornél\\codecool\\4_.NET\\2_TW\\sanity-archiver-csharp-cockroachy-salts\\SanityArchiver\\SanityArchiver.DesktopUI\\bin\\Debug" +
                "\\" + FileName.Text + ".zip";
            var targetLocation = _files[0].DirectoryPath + "/" + FileName.Text + ".zip";

            System.IO.File.Move(sourceLocation, targetLocation);
            Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            CompressTheFiles();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Vm = new CompressWindowViewModel();
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var binding = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);
            binding?.UpdateSource();
        }
    }
}