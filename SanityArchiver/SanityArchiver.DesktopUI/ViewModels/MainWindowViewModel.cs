using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SanityArchiver.DesktopUI.Models;
using SanityArchiver.DesktopUI.Views;

namespace SanityArchiver.DesktopUI.ViewModels
{
    public class MainWindowViewModel : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// Sets a List of Directory as ObservableCollection
        /// </summary>
        /// <param name="dirs"> List of directories</param>
        public MainWindowViewModel(List<Directory> dirs)
        {
            this.Directories = new ObservableCollection<Directory>(dirs);
            this.Files = new ObservableCollection<File>();
        }

        /// <summary>
        /// It's a collection of Directories
        /// </summary>
        public ObservableCollection<Directory> Directories
        {
            get => (ObservableCollection<Directory>)GetValue(DirectoriesProperty);
            set => SetValue(DirectoriesProperty, value);
        }

        public ObservableCollection<File> Files
        {
            get => (ObservableCollection<File>)GetValue(FilesProperty);
            set => SetValue(FilesProperty, value);
        }

        /// <summary>
        /// Registers Directories as DependencyProperty
        /// </summary>
        public static readonly DependencyProperty DirectoriesProperty =
            DependencyProperty.Register("Directories", typeof(ObservableCollection<Directory>), typeof(Views.MainWindow), new UIPropertyMetadata(null));

        public static readonly DependencyProperty FilesProperty =
            DependencyProperty.Register("Files", typeof(ObservableCollection<File>), typeof(Views.MainWindow), new UIPropertyMetadata(null));
    }
}
