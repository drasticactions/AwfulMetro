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
using System.Threading.Tasks;
using AwfulMetro.Common;
using AwfulMetro.Core.Manager;
using AwfulMetro.Pcl.Core.Exceptions;

namespace AwfulMetro.ViewModels
{
    public class LoginPageViewModel : NotifierBase
    {
        private readonly IAuthenticationManager _authManager;
        private string _password;
        private string _userName;
        private bool _isLoading;

        public LoginPageViewModel(IAuthenticationManager authManager)
        {
            ClickLoginButtonCommand = new AsyncDelegateCommand(async o => { await ClickLoginButton(); },
                o => CanClickLoginButton);
            _authManager = authManager;
        }

        public LoginPageViewModel() : this(new AuthenticationManager())
        {
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading == value) return;
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public string UserName
        {
            get { return _userName; }
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    OnPropertyChanged();
                    ClickLoginButtonCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged();
                    ClickLoginButtonCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool CanClickLoginButton
        {
            get { return !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password); }
        }

        public AsyncDelegateCommand ClickLoginButtonCommand { get; private set; }
        public event EventHandler<EventArgs> LoginSuccessful;
        public event EventHandler<EventArgs> LoginFailed;

        public async Task ClickLoginButton()
        {
            bool loginResult;
            IsLoading = true;
            try
            {
                loginResult = await _authManager.Authenticate(UserName, Password);
            }
            catch (LoginFailedException)
            {
                // TODO: The failure message should be bubbled up to the user; this is a different scenario than login failing due to bad credentials, and they should know that
                loginResult = false;
            }
            IsLoading = false;
            base.RaiseEvent(loginResult ? LoginSuccessful : LoginFailed, EventArgs.Empty);
        }
    }
}