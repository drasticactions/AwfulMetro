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