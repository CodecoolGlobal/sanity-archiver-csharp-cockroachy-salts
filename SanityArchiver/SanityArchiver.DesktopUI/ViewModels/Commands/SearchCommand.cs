using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SanityArchiver.DesktopUI.ViewModels.Commands
{
    public class SearchCommand : ICommand
    {
        static public MainWindowViewModel MainWindowViewModel { get; set; }
        public SearchCommand(MainWindowViewModel mainWindowViewModel)
        {
            MainWindowViewModel = mainWindowViewModel;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            MainWindowViewModel.SearchFile();
        }

        public event EventHandler CanExecuteChanged;
    }
}
