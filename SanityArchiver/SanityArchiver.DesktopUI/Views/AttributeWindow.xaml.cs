using SanityArchiver.Application.Models;
using SanityArchiver.DesktopUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SanityArchiver.DesktopUI.Views
{
    /// <summary>
    /// Interaction logic for AttributeWindow.xaml
    /// </summary>
    public partial class AttributeWindow : Window
    {

        private AttributeWindowViewModel _vm;

        /// <summary>
        ///
        /// </summary>
        public AttributeWindowViewModel Vm
        {
            get => _vm;
            set
            {
                _vm = value;
                DataContext = _vm;
            }
        }

        private File _file;


        public AttributeWindow(File file)
        {
            _file = file;
            file.FileName = CutExtensionFromFileName(file.FileName);
            InitializeComponent();
            AttributeListView.DataContext = file;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public string CutExtensionFromFileName(string fileName)
        {
            int fileExtPos = fileName.LastIndexOf(".", StringComparison.Ordinal);
            return fileExtPos >= 0 ? fileName.Substring(0, fileExtPos) : fileName;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Vm = new AttributeWindowViewModel();
        }

        private void TextBoxFileName_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var binding = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);
            Console.WriteLine(binding.ToString());
            binding?.UpdateSource();
        }

        private void TextBoxFileExtension_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var binding = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);
            binding.ToString();
            binding?.UpdateSource();
        }

        private void TextBoxIsHidden_OnTextChanged(object sender, EventArgs e)
        {
            var binding = ((CheckBox)sender).GetBindingExpression(CheckBox.IsCheckedProperty);
            binding.ToString();
            binding?.UpdateSource();
        }

        private void AttribSaveButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
