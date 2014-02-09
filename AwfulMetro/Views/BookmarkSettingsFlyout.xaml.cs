using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using AwfulMetro.Core.Tools;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace AwfulMetro.Views
{
    public sealed partial class BookmarkSettingsFlyout : SettingsFlyout
    {
        private readonly ApplicationDataContainer _localSettings;

        public BookmarkSettingsFlyout()
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
    }
}