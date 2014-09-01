#region copyright
/*
Awful Metro - A Modern UI Something Awful Forums Viewer
Copyright (C) 2014  Tim Miller

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software Foundation,
Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301  USA
*/
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace AwfulMetro.Commands
{
    public interface IThreadCommand : ICommand
    {
        void RaiseCanExecuteChanged();
    }

    public class ThreadCommand : IThreadCommand
    {
        readonly Action<object> _execute;
        readonly Func<object, bool> _canExecute;

        #region Constructors
        public ThreadCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            this._execute = execute;
            this._canExecute = canExecute;
        }

        public ThreadCommand(Action<object> execute)
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
