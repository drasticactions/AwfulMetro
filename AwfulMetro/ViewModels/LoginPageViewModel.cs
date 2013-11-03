using System;
using System.Threading.Tasks;
using AwfulMetro.Common;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Manager;

namespace AwfulMetro.ViewModels
{
    public class LoginPageViewModel : NotifierBase
    {
        public event EventHandler<EventArgs> LoginSuccessful;
        public event EventHandler<EventArgs> LoginFailed;

        private readonly IAuthenticationManager _authManager;
        private string _userName;
        private string _password;

        public string UserName
        {
            get
            {
                return this._userName;
            }
            set
            {
                if (this._userName != value)
                {
                    this._userName = value;
                    this.OnPropertyChanged();
                    ClickLoginButtonCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string Password
        {
            get
            {
                return this._password;
            }
            set
            {
                if (this._password != value)
                {
                    this._password = value;
                    this.OnPropertyChanged();
                    ClickLoginButtonCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool CanClickLoginButton 
        {
            get
            {
                return !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password);
            } 
        }

        public AsyncDelegateCommand ClickLoginButtonCommand { get; private set; }

        public LoginPageViewModel(IAuthenticationManager authManager)
        {
            ClickLoginButtonCommand = new AsyncDelegateCommand(async o => { await this.ClickLoginButton(); }, o => CanClickLoginButton);
            this._authManager = authManager;
        }

        public LoginPageViewModel() : this(new AuthenticationManager())
        {
            
        }

        public async Task ClickLoginButton()
        {
            bool loginResult;
            try
            {
                loginResult = await this._authManager.Authenticate(this.UserName, this.Password);
            }
            catch (LoginFailedException)
            {
                // TODO: The failure message should be bubbled up to the user; this is a different scenario than login failing due to bad credentials, and they should know that
                loginResult = false;
            }

            if (loginResult)
            {
                base.RaiseEvent(LoginSuccessful, EventArgs.Empty);
            }
            else
            {
                base.RaiseEvent(LoginFailed, EventArgs.Empty);
            }
        }
    }
}
