using System;
using System.IO;
using System.Windows;
using SanityArchiver.DesktopUI.ViewModels;

namespace SanityArchiver.DesktopUI.Views
{
    /// <summary>
    /// Interaction logic for FileAttributeWindow.xaml
    /// </summary>
    public partial class FileAttributeWindow
    {
        private FileAttributeWindowViewModel _vm;

        /// <summary>
        ///
        /// </summary>
        public FileAttributeWindowViewModel Vm
        {
            get => _vm;
            set
            {
                _vm = value;
                DataContext = _vm;
            }
        }

        public DirectoryItemViewModel File;

        public FileAttributeWindow(DirectoryItemViewModel file)

        {
            InitializeComponent();
            File = file;
            Vm = new FileAttributeWindowViewModel
            {
                Extension = File.Extension,
                FileName = File.Name.Substring(0, File.Name.Length - File.Extension.Length),
                IsHidden = File.IsHidden,
                SaveBtnEnabled = false,
            };
            AttributeListView.DataContext = Vm;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            FileAttributes attributes = System.IO.File.GetAttributes(File.FullPath);
            if (!Vm.IsHidden)
            {
                attributes = RemoveAttribute(attributes, FileAttributes.Hidden);
                System.IO.File.SetAttributes(File.FullPath, attributes);
            }
            else
            {
                System.IO.File.SetAttributes(File.FullPath,
                    System.IO.File.GetAttributes(File.FullPath) | FileAttributes.Hidden);
            }

            string newPath =
                $"{File.FullPath.Substring(0, File.FullPath.Length - File.Name.Length)}{Vm.FileName}{File.Extension}";
            System.IO.File.Move(File.FullPath, newPath);
            System.IO.File.Move(newPath, Path.ChangeExtension(newPath, Vm.Extension));
            Close();
        }

        private static FileAttributes RemoveAttribute(FileAttributes attributes, FileAttributes attributesToRemove)
        {
            return attributes & ~attributesToRemove;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AttribHidden.IsChecked = Vm.IsHidden;
        }
    }
}