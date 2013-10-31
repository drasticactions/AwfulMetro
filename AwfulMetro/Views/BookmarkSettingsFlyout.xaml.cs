using BusinessObjects.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace AwfulMetro.Views
{
    public sealed partial class BookmarkSettingsFlyout : SettingsFlyout
    {

        ApplicationDataContainer localSettings;

        public BookmarkSettingsFlyout()
        {
            this.InitializeComponent();
            localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if(localSettings.Values.ContainsKey(Constants.BOOKMARK_BACKGROUND))
            {
                BookmarkLiveTiles.IsOn = (bool)localSettings.Values[Constants.BOOKMARK_BACKGROUND];
            }
        }

        private void BookmarkSelectionMode_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BookmarkLiveTiles_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
               
                if (toggleSwitch.IsOn == true)
                {
                    // Run bookmark live tile creator every 15 minutes.
                    // TODO: Change 15 to user selectable value.
                    var task = BackgroundTaskUtils.RegisterBackgroundTask(BackgroundTaskUtils.BackgroundTaskEntryPoint,
                                                                BackgroundTaskUtils.BackgroundTaskName,
                                                                new TimeTrigger(15, false),
                                                            null);
                    localSettings.Values[Constants.BOOKMARK_BACKGROUND] = true;
                }
                else
                {
                    BackgroundTaskUtils.UnregisterBackgroundTasks(BackgroundTaskUtils.BackgroundTaskName);
                    localSettings.Values[Constants.BOOKMARK_BACKGROUND] = false;
                }
            }
        }
    }
}
