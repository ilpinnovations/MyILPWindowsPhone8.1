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
using MyILP.Code;
using System.Net.Http;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace MyILP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BadgePage : Page
    {
        public BadgePage()
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

            ProcessGetLeaderboardUI();
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            menu.RemoveMenuSlider();
        }

        private void ShowProgressBar(bool show)
        {
            progressBar.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }
        private async void ProcessGetLeaderboardUI()
        {
            ShowProgressBar(true);

            try
            {
                var leaderBoardModel = await MyILPClient.GetLeaderboard();
                tbMyName.Text = UserInformation.FullName;
                tbMyBatch.Text = UserInformation.LGName;
                tbMyLevel.Text = AchievementsHelper.GetAchievementString(leaderBoardModel.MyPoints);
                gridMain.DataContext = leaderBoardModel;

            }
            catch (HttpRequestException exception)
            {
                tbMyBatch.Text = string.Empty;
                tbMyLevel.Text = "No internet connection";
                (new MessageDialog(exception.Message, "Error")).ShowAsync().GetResults();
            }            
            finally
            {
                ShowProgressBar(false);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //help button clicked
            string title = "Information";
            string content = "You can earn points by giving feedbacks to your faculty in the schedule.";
            (new MessageDialog(content, title)).ShowAsync().GetResults();
        }
    }
}
