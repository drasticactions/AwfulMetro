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
using Windows.UI.Popups;
using AwfulMetro.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556
using AwfulMetro.ViewModels;

namespace AwfulMetro.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page, IDisposable
    {
        public LoginPage()
        {
            InitializeComponent();
            var vm = (LoginPageViewModel)DataContext;
            vm.LoginFailed += OnLoginFailed;
            vm.LoginSuccessful += OnLoginSuccessful;
        }

        public void Dispose()
        {
            var vm = DataContext as LoginPageViewModel;
            if (vm == null) return;
            vm.LoginFailed -= OnLoginFailed;
            vm.LoginSuccessful -= OnLoginSuccessful;
        }

        private void OnLoginSuccessful(object sender, EventArgs e)
        {
            Frame.Navigate(typeof(MainForumsPage));
        }

        private static async void OnLoginFailed(object sender, EventArgs e)
        {
            var msg = new MessageDialog("ERROR: Your Username/Password were incorrect, try again", "Alert");
            await msg.ShowAsync();
        }
    }
}
