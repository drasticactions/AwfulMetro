using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace AwfulMetro.Commands
{
    public interface IUnreadCommand : ICommand
    {
        void RaiseCanExecuteChanged();
    }

    public class UnreadCommand : IUnreadCommand
    {
        readonly Action<object> _execute;
        readonly Func<object, bool> _canExecute;

        #region Constructors
        public UnreadCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            this._execute = execute;
            this._canExecute = canExecute;
        }

        public UnreadCommand(Action<object> execute)
        {
            this._execute = execute;
            this._canExecute = this.AlwaysCanExecute;
        } 
        #endregion
        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        } 

        public bool CanExecute(object parameter)
        {
            return _canExecute(parameter); 
        }

        public void Execute(object parameter)
        {
            _execute(parameter); 
        }

        public event EventHandler CanExecuteChanged;

        bool AlwaysCanExecute(object param)
        {
            return true;
        } 
    }
}
