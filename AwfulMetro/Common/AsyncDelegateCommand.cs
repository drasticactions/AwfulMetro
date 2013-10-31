using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AwfulMetro.Common
{
    public sealed class AsyncDelegateCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Func<object, Task> _asyncExecute;

        public event EventHandler CanExecuteChanged;

        public AsyncDelegateCommand(Func<object, Task> execute)
            : this(execute, null)
        {
        }

        public AsyncDelegateCommand(Func<object, Task> asyncExecute,
                       Predicate<object> canExecute)
        {
            _asyncExecute = asyncExecute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            return _canExecute(parameter);
        }

        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        private async Task ExecuteAsync(object parameter)
        {
            await _asyncExecute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            var handler = this.CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
