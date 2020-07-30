using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using SanityArchiver.DesktopUI.ViewModels;

namespace SanityArchiver.DesktopUI.Views
{
    public partial class CompressWindow
    {
        private CompressWindowViewModel _vm;

        public CompressWindowViewModel Vm
        {
            get => _vm;
            set
            {
                _vm = value;
                DataContext = _vm;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Vm.FileName = FileName.Text;
            Vm.CompressTheFiles();
            Close();
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var binding = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);
            binding?.UpdateSource();
        }

        public CompressWindow(List<DirectoryItemViewModel> files)
        {
            InitializeComponent();
            Vm = new CompressWindowViewModel { Files = files };
        }
    }
}