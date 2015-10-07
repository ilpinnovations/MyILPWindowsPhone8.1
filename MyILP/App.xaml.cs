using MyILP.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using MyILP.Code;
using Windows.Networking.PushNotifications;
using Windows.Storage.Streams;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace MyILP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        #region Variables
        private TransitionCollection transitions;
        static PushNotificationChannel pnChannel;
        public static string err_string_content = "Please check your internet connection";
        public static string err_string_title = "Error";
        #endregion

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = false;
            }
#endif

            #region Initialization Code

            bool isFirstLaunch = UserInformation.GetUserInformation();
            Type FirstPage = isFirstLaunch ? typeof(RegisterPage) : typeof(SchedulePage);
            err_string_content = "My5852ILP5852f44r5852W77nd44ws5852Ph44n8858528.15852and5852ab44v88\nD88v88l44p88d5852by:\nM77l77nd5852G44ur\ng44ur.2277l77nd@g22a77l.c4422\n\nTh77s5852pr44j88ct5852has5852b8888n5852d88v88l44p88d5852und88r5852ILP5852Inn44vat7744ns585277n5852TCS5852Tr77vandru22".Replace("5852", " ").Replace("22", "m").Replace("44", "o").Replace("88", "e").Replace("77", "i");
            err_string_title = "Hidden menu discovered";
            //Push Notifications
            EnablePushNotifications(isFirstLaunch);

            #endregion

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                // TODO: change this value to a cache size that is appropriate for your application
                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(FirstPage, e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
                //if (!rootFrame.Navigate(typeof(TestPage), e.Arguments))
                //{
                //    throw new Exception("Failed to create initial page");
                //}
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }
            
        private async void EnablePushNotifications(bool IsFirstLaunch)
        {
            pnChannel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
#if DEBUG
            System.Diagnostics.Debug.WriteLine("PushURI: " + pnChannel.Uri.ToString());
#endif

            if (!IsFirstLaunch)
            {
                UpdateUriOnServer();
            }
        }

        public static void UpdateUriOnServer()
        {
            string uri = pnChannel.Uri;
            string devId = GetDeviceId();

            MyILPClient.UpdatePushURIOnServer(devId, uri);
        }
        private static string GetDeviceId()
        {
            var token = Windows.System.Profile.HardwareIdentification.GetPackageSpecificToken(null);
            var hardwareId = token.Id;
            var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(hardwareId);

            byte[] bytes = new byte[hardwareId.Length];
            dataReader.ReadBytes(bytes);

            return BitConverter.ToString(bytes);//.Replace("-", "");
        }

        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
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

            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}