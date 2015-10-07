using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace MyILP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NotificationPage : Page
    {
        public NotificationPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            menu.InstallMenuSlider(this.Frame);

            ProcessGetNotificationsUI();
        }
        private async void ProcessGetNotificationsUI()
        {
            ShowProgressBar(true);

            var notificationModel = await MyILP.Code.MyILPClient.GetNotifications();

            if (notificationModel.NotificationItems.Count == 0)
            {
                string msg = "No notifications available right now.";
                var response = await new Windows.UI.Popups.MessageDialog(msg).ShowAsync();
            }

            listNotifications.DataContext = notificationModel.NotificationItems;
            ShowProgressBar(false);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            menu.RemoveMenuSlider();
        }

        private void ShowProgressBar(bool show)
        {
            progressBar.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
