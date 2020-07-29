using SanityArchiver.Application.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SanityArchiver.DesktopUI.Views
{
    /// <summary>
    /// Interaction logic for EncryptWindow.xaml
    /// </summary>
    public partial class EncryptWindow : Window
    {
        private List<File> FilesToEncrypt;   

        public EncryptWindow(List<File> files)
        {
            FilesToEncrypt = files;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EncryptFiles();
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void EncryptFiles()
        {
            foreach (var file in FilesToEncrypt)
            {
                System.IO.File.Encrypt(file.FullPath);
                ChangeFileExtension(FilesToEncrypt, ".ENC");
                FilesToEncrypt = new List<File>();
            }

        }


        public void ChangeFileExtension(IEnumerable<File> filesToEncrypt, string extension)
        {
            foreach (var file in filesToEncrypt)
            {
                System.IO.File.Move(
                    file.FullPath,
                    System.IO.Path.ChangeExtension(file.FullPath, extension) ?? throw new InvalidOperationException());
            }
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var binding = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);
            binding?.UpdateSource();
        }
    }
}
