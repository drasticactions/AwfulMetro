using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace AwfulMetro.Common
{
    public abstract class AlwaysExecutableCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public abstract void Execute(object parameter);

        public event EventHandler CanExecuteChanged;
    }
}
