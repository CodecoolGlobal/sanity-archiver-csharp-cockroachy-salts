using System;
using System.Windows.Input;

namespace SanityArchiver.DesktopUI.ViewModels.Commands
{
    /// <summary>
    ///
    /// </summary>
    public class SearchCommand : ICommand
    {
        /// <summary>
        ///
        /// </summary>
        public static MainWindowViewModel MainWindowViewModel { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchCommand"/> class.
        /// </summary>
        /// <param name="mainWindowViewModel"></param>
        public SearchCommand(MainWindowViewModel mainWindowViewModel)
        {
            MainWindowViewModel = mainWindowViewModel;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            MainWindowViewModel.SearchFile();
        }

        /// <summary>
        ///
        /// </summary>
        public event EventHandler CanExecuteChanged;
    }
}