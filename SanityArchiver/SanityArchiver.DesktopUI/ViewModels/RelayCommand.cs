using System;
using System.Windows.Input;

namespace SanityArchiver.DesktopUI.ViewModels
{
    public class RelayCommand : ICommand
    {
        #region Private Members

        private Action _mAction;

        #endregion

        #region Public Events

        public event EventHandler CanExecuteChanged = (sender, e) => { };

        #endregion

        #region Constructor

        public RelayCommand(Action action)
        {
            _mAction = action;
        }

        #endregion

        #region Command Methods

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _mAction();
        }

        #endregion
    }
}