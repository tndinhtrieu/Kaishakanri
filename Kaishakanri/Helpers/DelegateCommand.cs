using System;
using System.Windows.Input;

namespace Kaishakanri.Helpers
{
    public class DelegateCommand : ICommand
    {
        private readonly Action _command;
        private readonly Action<object> _commandHasPara;
        private readonly Func<bool> _canExecute;
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DelegateCommand(Action command, Func<bool> canExecute = null)
        {
            if (command == null)
                throw new ArgumentNullException();
            _canExecute = canExecute;
            _command = command;
        }

        public DelegateCommand(Action<object> command, Func<bool> canExecute = null)
        {
            if (command == null)
                throw new ArgumentNullException();
            _canExecute = canExecute;
            _commandHasPara = command;
        }

        public void Execute(object parameter)
        {
            if (parameter == null)
                _command();
            else
                _commandHasPara(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

    }
}