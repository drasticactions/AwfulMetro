using System;
using System.Diagnostics;
using System.Net;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.System;
using Windows.UI.ApplicationSettings;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using AwfulMetro.Common;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Manager;
using AwfulMetro.Core.Tools;
using AwfulMetro.Views;

// The Grid App template is documented at http://go.microsoft.com/fwlink/?LinkId=234226
using Newtonsoft.Json;

namespace AwfulMetro
{
    /// <summary>
    ///     Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        ///     Initializes the singleton Application object.  This is the first line of authored code
        ///     executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        /// <summary>
        ///     Invoked when the application is launched normally by the end user.  Other entry points
        ///     will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            // Show graphics profiling information while debugging.
            if (Debugger.IsAttached)
            {
                // Display the current frame rate counters
                DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            var rootFrame = Window.Current.Content as Frame;
            SettingsPane.GetForCurrentView().CommandsRequested += SettingCharmManager_CommandsRequested;
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Clear();
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                //Associate the frame with a SuspensionManager key                                
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        //Something went wrong restoring state.
                        //Assume there is no state and continue
                    }
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }
            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                var localStorageManager = new LocalStorageManager();
                CookieContainer cookieTest = await localStorageManager.LoadCookie(Constants.COOKIE_FILE);
                if (cookieTest.Count <= 0)
                {
                    if (!rootFrame.Navigate(typeof (LoginPage)))
                    {
                        throw new Exception("Failed to create initial page");
                    }
                }
                else
                {
                    var localSettings = ApplicationData.Current.LocalSettings;
                    if (localSettings.Values.ContainsKey(Constants.BOOKMARK_STARTUP) && (bool)localSettings.Values[Constants.BOOKMARK_STARTUP])
                    {
                            var forum = new ForumEntity("Bookmarks", Constants.USER_CP, string.Empty, false);
                            string jsonObjectString = JsonConvert.SerializeObject(forum);
                            rootFrame.Navigate(typeof(ThreadListPage), jsonObjectString);
                    }
                    else
                    {
                        if (!rootFrame.Navigate(typeof(MainForumsPage)))
                        {
                            throw new Exception("Failed to create initial page");
                        }
                    }
                }
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        private void SettingCharmManager_CommandsRequested(SettingsPane sender,
            SettingsPaneCommandsRequestedEventArgs args)
        {
            args.Request.ApplicationCommands.Add(new SettingsCommand("privacypolicy", "Privacy Policy",
                OpenPrivacyPolicy));
        }

        private async void OpenPrivacyPolicy(IUICommand command)
        {
            var uri = new Uri("https://sites.google.com/site/awfulforumsreader/");
            await Launcher.LaunchUriAsync(uri);
        }

        /// <summary>
        ///     Invoked when application execution is being suspended.  Application state is saved
        ///     without knowing whether the application will be terminated or resumed with the contents
        ///     of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }
    }
}