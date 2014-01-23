// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
using System;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using AwfulMetro.ViewModels;

namespace AwfulMetro.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page, IDisposable
    {
        public LoginPage()
        {
            InitializeComponent();
            var vm = (LoginPageViewModel) DataContext;
            vm.LoginFailed += OnLoginFailed;
            vm.LoginSuccessful += OnLoginSuccessful;
        }

        public void Dispose()
        {
            var vm = DataContext as LoginPageViewModel;
            if (vm != null)
            {
                vm.LoginFailed -= OnLoginFailed;
                vm.LoginSuccessful -= OnLoginSuccessful;
            }
        }

        private void OnLoginSuccessful(object sender, EventArgs e)
        {
            Frame.Navigate(typeof (MainForumsPage));
        }

        private static async void OnLoginFailed(object sender, EventArgs e)
        {
            var msg = new MessageDialog("ERROR: Your Username/Password were incorrect, try again", "Alert");
            await msg.ShowAsync();
        }
    }
}