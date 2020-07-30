using System;
using System.Windows;
using SanityArchiver.DesktopUI.ViewModels;

namespace SanityArchiver.DesktopUI.Views
{
    /// <summary>
    /// Interaction logic for EncryptWindow.xaml
    /// </summary>
    public partial class EncryptWindow
    {
        private DirectoryItemViewModel _file;

        public EncryptWindow(DirectoryItemViewModel file)
        {
            _file = file;
            InitializeComponent();
        }

        public void EncryptButton_Click(object sender, RoutedEventArgs e)
        {
            System.IO.File.Encrypt(_file.FullPath);
            System.IO.File.Move(
                _file.FullPath,
                System.IO.Path.ChangeExtension(_file.FullPath, ".ENC") ?? throw new InvalidOperationException());
            Close();
        }

        public void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}