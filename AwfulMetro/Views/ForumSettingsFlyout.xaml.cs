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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using AwfulMetro.Core.Tools;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769
using AwfulMetro.Pcl.Core.Tools;

namespace AwfulMetro.Views
{
    public sealed partial class ForumSettingsFlyout
    {
        private readonly ApplicationDataContainer _localSettings;

        public ForumSettingsFlyout()
        {
            InitializeComponent();
            _localSettings = ApplicationData.Current.LocalSettings;
            if (_localSettings.Values.ContainsKey(Constants.BOOKMARK_BACKGROUND))
            {
                BookmarkLiveTiles.IsOn = (bool) _localSettings.Values[Constants.BOOKMARK_BACKGROUND];
            }
            if (_localSettings.Values.ContainsKey(Constants.BOOKMARK_STARTUP))
            {
                LoadBookmarksOnLoadSwitch.IsOn = (bool)_localSettings.Values[Constants.BOOKMARK_STARTUP];
            }
            if (_localSettings.Values.ContainsKey(Constants.BOOKMARK_DEFAULT))
            {
                FilterComboBox.SelectedIndex = (int) _localSettings.Values[Constants.BOOKMARK_DEFAULT];
            }
            else
            {
                FilterComboBox.SelectedIndex = 0;
            }
            if (_localSettings.Values.ContainsKey(Constants.DARK_MODE))
            {
                DarkLightThemeSwitch.IsOn = (bool)_localSettings.Values[Constants.DARK_MODE];
            }
            if (_localSettings.Values.ContainsKey(Constants.AUTO_REFRESH))
            {
                AutoReloadSwitch.IsOn = (bool)_localSettings.Values[Constants.AUTO_REFRESH];
            }
        }

        private void BookmarkSelectionMode_Click(object sender, RoutedEventArgs e)
        {
        }

        private async void BookmarkLiveTiles_Toggled(object sender, RoutedEventArgs e)
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

        private void FilterComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilterComboBox == null) return;
            // TODO: Make Enum.
            _localSettings.Values[Constants.BOOKMARK_DEFAULT] = FilterComboBox.SelectedIndex;
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

        private void AutoReloadSwitch_OnToggled(object sender, RoutedEventArgs e)
        {
            var toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch == null) return;
            if (toggleSwitch.IsOn)
            {
                _localSettings.Values[Constants.AUTO_REFRESH] = true;
            }
            else
            {
                _localSettings.Values[Constants.AUTO_REFRESH] = false;
            }
        }
    }
}