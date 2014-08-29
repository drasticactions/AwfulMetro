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
using Windows.ApplicationModel.Background;
using Windows.Storage;
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
using AwfulMetro.Core.Tools;
using AwfulMetro.Pcl.Core.Tools;

namespace AwfulMetro.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly ApplicationDataContainer _localSettings;
        public SettingsPage()
        {
            this.InitializeComponent();
            _localSettings = ApplicationData.Current.LocalSettings;
            if (_localSettings.Values.ContainsKey(Constants.BOOKMARK_BACKGROUND))
            {
                BookmarkLiveTiles.IsOn = (bool)_localSettings.Values[Constants.BOOKMARK_BACKGROUND];
            }
            if (_localSettings.Values.ContainsKey(Constants.BOOKMARK_STARTUP))
            {
                LoadBookmarksOnLoadSwitch.IsOn = (bool)_localSettings.Values[Constants.BOOKMARK_STARTUP];
            }
            if (_localSettings.Values.ContainsKey(Constants.DARK_MODE))
            {
                DarkLightThemeSwitch.IsOn = (bool) _localSettings.Values[Constants.DARK_MODE];
            }
            else
            {
                // If the mode was never set, it's automatically true unless the user actually sets it to false.
                DarkLightThemeSwitch.IsOn = true;
            }
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void LoadBookmarksOnLoadSwitch_OnToggled(object sender, RoutedEventArgs e)
        {
            var toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch == null) return;
            if (toggleSwitch.IsOn)
            {
                _localSettings.Values[Constants.BOOKMARK_STARTUP] = true;
            }
            else
            {
                _localSettings.Values[Constants.BOOKMARK_STARTUP] = false;
            }
        }

        private async void BookmarkLiveTiles_Toggled(object sender, RoutedEventArgs e)
        {
            try
            {
                var toggleSwitch = sender as ToggleSwitch;
                if (toggleSwitch == null) return;
                if (toggleSwitch.IsOn)
                {
                    // Run bookmark live tile creator every 15 minutes.
                    // TODO: Change 15 to user selectable value.
                    BackgroundTaskUtils.UnregisterBackgroundTasks(BackgroundTaskUtils.BackgroundTaskName);
                    BackgroundTaskRegistration task = await
                        BackgroundTaskUtils.RegisterBackgroundTask(BackgroundTaskUtils.BackgroundTaskEntryPoint,
                            BackgroundTaskUtils.BackgroundTaskName,
                            new TimeTrigger(15, false),
                            null);
                    _localSettings.Values[Constants.BOOKMARK_BACKGROUND] = true;
                }
                else
                {
                    BackgroundTaskUtils.UnregisterBackgroundTasks(BackgroundTaskUtils.BackgroundTaskName);
                    _localSettings.Values[Constants.BOOKMARK_BACKGROUND] = false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void DarkLightThemeSwitch_OnToggled(object sender, RoutedEventArgs e)
        {
            var toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch == null) return;
            if (toggleSwitch.IsOn)
            {
                _localSettings.Values[Constants.DARK_MODE] = true;
            }
            else
            {
                _localSettings.Values[Constants.DARK_MODE] = false;
            }
        }
    }
}
