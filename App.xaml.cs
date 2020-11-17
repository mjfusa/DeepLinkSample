using Microsoft.Services.Store.Engagement;
using Microsoft.WindowsAzure.Messaging;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Networking.PushNotifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Toolkit.Uwp.Helpers;
//using Microsoft.Toolkit.Uwp.Helpers;

namespace DeepLinkSample
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            AppCenter.Start("xxxxx-4ec8-4c3e-96a4-b63ec5839c36",
                   typeof(Analytics), typeof(Crashes));
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            if (args is ToastNotificationActivatedEventArgs)
            {
                Analytics.TrackEvent("User Clicked Notification");

                var toastActivationArgs = args as ToastNotificationActivatedEventArgs;

                StoreServicesEngagementManager engagementManager = StoreServicesEngagementManager.GetDefault();
                string originalArgs = engagementManager.ParseArgumentsAndTrackAppLaunch(
                    toastActivationArgs.Argument);

                // Use the originalArgs variable to access the original arguments
                // that were passed to the app.
            }

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            var navigationDestination = typeof(MainPage);
            if (rootFrame.Content == null)
            {
                rootFrame.Navigate(navigationDestination);
            }
            if (args.Kind == ActivationKind.Protocol)
            {
                ProtocolActivatedEventArgs eventArgs = args as ProtocolActivatedEventArgs;
                var parser = DeepLinkParser.Create(args);
                if (parser.ContainsKey("promo"))
                {
                    if (parser["promo"] == "winterwonderland")
                    {
                        navigationDestination = typeof(PromoPage);
                        rootFrame.Navigate(navigationDestination);
                    }
                }
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                InitNotificationsAsync();

                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            var navigationDestination = typeof(MainPage);
            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    if (rootFrame.Content == null)
                    {
                        // When the navigation stack isn't restored navigate to the first page,
                        // configuring the new page by passing required information as a navigation
                        // parameter
                        rootFrame.Navigate(typeof(MainPage), e.Arguments);
                    }
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        private async void InitNotificationsAsync()
        {
            PushNotificationChannel pnc = null;
            Registration result = null;
            //try
            //{
            //    var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();

            //    var hub = new NotificationHub("wnsTestHub", "Endpoint=sb://wnstest.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=xxxxxxxxIM9oJOAEpekrKLCYGNnHtpjEULOLv01G0Sbi8=");
            //    var res1 = await hub.RegisterNativeAsync(channel.Uri);
            //    pnc = channel;
            //    result = res1;
            //}
            //catch (Exception ex)
            //{
            //    Crashes.TrackError(ex);
            //}

#if DEBUG
            // Displays the registration ID so you know it was successful
            //if (result != null)
            //    if (result.RegistrationId != null)
            //    {
            //        var dialog = new MessageDialog("Notification Hub Registration successful: " + result.RegistrationId);
            //        dialog.Commands.Add(new UICommand("OK"));
            //        await dialog.ShowAsync();
            //    }
#endif
            StoreServicesNotificationChannelRegistrationResult res = null;
            try
            {
                // Register with Partner Center engagement notifications
                StoreServicesNotificationChannelParameters parameters = new StoreServicesNotificationChannelParameters();
                if (pnc != null)
                {
                    parameters.CustomNotificationChannelUri = pnc.Uri;
                }

                StoreServicesEngagementManager engagementManager = StoreServicesEngagementManager.GetDefault();
                res = await engagementManager.RegisterNotificationChannelAsync(parameters);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }


#if DEBUG
            // Displays the registration ID so you know it was successful
            if (res.ErrorCode == StoreServicesEngagementErrorCode.None)
            {
                var dialog = new MessageDialog("Engagement Notificaiton Registration successful: " + res.ErrorMessage);
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
            }
#endif
        }

    }
}
