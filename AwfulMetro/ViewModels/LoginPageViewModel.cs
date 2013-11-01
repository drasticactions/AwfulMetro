using System;
using System.Threading.Tasks;
using AwfulMetro.Common;
using AwfulMetro.Core.Manager;

namespace AwfulMetro.ViewModels
{
    public class LoginPageViewModel : NotifierBase
    {
        public event EventHandler<EventArgs> LoginSuccessful;
        public event EventHandler<EventArgs> LoginFailed;

        private readonly IAuthenticationManager authManager;
        private string userName;
        private string password;

        public string UserName
        {
            get
            {
                return userName;
            }
            set
            {
                if (userName != value)
                {
                    userName = value;
                    this.OnPropertyChanged();
                    ClickLoginButtonCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                if (password != value)
                {
                    password = value;
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
            this.authManager = authManager;
        }

        public LoginPageViewModel() : this(new AuthenticationManager())
        {
            
        }

        public async Task ClickLoginButton()
        {
            bool loginResult;
            try
            {
                loginResult = await authManager.Authenticate(this.UserName, this.Password);
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
