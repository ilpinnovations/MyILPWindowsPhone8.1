using MyILP.Code;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI.Popups;
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
    public sealed partial class SchedulePage : Page
    {
        ScheduleItem currentSelectedSchedule = null;
        public SchedulePage()
        {
            this.InitializeComponent();
            feedControl.FeedbackCancelled += feedControl_FeedbackCancelled;
            feedControl.FeedbackReceived += feedControl_FeedbackReceived;
        }

        async void feedControl_FeedbackReceived(Code.FeedbackObject feedbackObj)
        {
            ShowProgressBar(true);

            // send the feedback to the server from here
            var feedbackResult = await MyILPClient.RegisterFacultyFeedback(currentSelectedSchedule, feedbackObj);

            string title = "Information";
            string content = "";
            switch (feedbackResult.FeedStatus)
            {
                case FeedbackResult.FeedbackStatusType.NEW:
                    content = "Thank you giving your valuable feedback!";
                    break;
                case FeedbackResult.FeedbackStatusType.OLD:
                    content = "Feedback has already been given for the selected slot";
                    break;
                case FeedbackResult.FeedbackStatusType.ERROR:
                    title = "Error";
                    content = "Some error has occured while trying to send feedback. Please try again later.";
                    break;
            }
            MessageDialog md = new MessageDialog(content, title);
            await md.ShowAsync();

#if DEBUG
            System.Diagnostics.Debug.WriteLine("Feedback result: " + feedbackResult.FeedStatus);
#endif
            ShowProgressBar(false);
        }

        void feedControl_FeedbackCancelled(Code.FeedbackObject feedbackObj)
        {
            ShowProgressBar(false);
        }

        void InitializeUI()
        {
            tbBatch.Text = MyILP.Code.UserInformation.LGName;
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

            InitializeUI();
            ProcessGetScheduleUI();
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            menu.RemoveMenuSlider();
        }

        private void ShowProgressBar(bool show)
        {
            progressBar.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            EnableControls(!show);
        }

        private void EnableControls(bool enable)
        {
            tbBatch.IsEnabled =
            datePicker.IsEnabled =
            listSchedule.IsEnabled =
            btnHelp.IsEnabled = 
            btnGetSchedule.IsEnabled = enable;

            menu.IsEnabled = enable;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Get-Schedule Clicked

            tbBatch.Text = tbBatch.Text.ToUpper();
            if (string.IsNullOrEmpty(tbBatch.Text))
            {
                (new MessageDialog("Please enter your batch in the given field", "Error")).ShowAsync().GetResults();
                return;
            }
            ProcessGetScheduleUI();
        }
        private async void ProcessGetScheduleUI(bool force = false)
        {
            ShowProgressBar(true);
            
            var date = datePicker.Date;
            string selectedDateString = string.Format("{0}-{1}-{2}", date.Year, date.Month, date.Day);
            string batchName = tbBatch.Text.ToUpper();

            ScheduleModel scheduleModel = null;

            try
            {
                if (force)
                {
                    scheduleModel = await MyILP.Code.MyILPClient.GetSchedule_NetworkPriority(batchName, selectedDateString);
                }
                else
                {
                    scheduleModel = await MyILP.Code.MyILPClient.GetSchedule(batchName, selectedDateString);
                }

            }
            catch (Exception)
            {
                
            }
            if (scheduleModel.ScheduleItems.Count == 0)
            {
                string msg = "No schedule returned from the server.";
                var response = await new Windows.UI.Popups.MessageDialog(msg).ShowAsync();
            }

            listSchedule.DataContext = scheduleModel.ScheduleItems;
            ShowProgressBar(false);
        }

        private void listSchedule_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView lv = sender as ListView;
            if (lv == null) return;
            if (lv.SelectedIndex == -1) return;

            currentSelectedSchedule = (Code.ScheduleItem)lv.SelectedItem;
            //ShowProgressBar(true);

            EnableControls(false);
            lv.SelectedIndex = -1;

            bool hasNoID;
            if (IsFacultyAccount(currentSelectedSchedule.Faculty, out hasNoID))
            {
                // initiate the feedback information fetch
                ProcessAvgRatingDialog();
            }
            else
            {
                if (hasNoID)
                {
                    (new MessageDialog("You can not give feedback for this slot", "Error")).ShowAsync().GetResults();
                    ShowProgressBar(false);
                    return;
                }

                DateTime today = DateTime.Now.Date;
                DateTime schedDate = DateTime.Parse(currentSelectedSchedule.Date1).Date;
                if (schedDate != today)
                {
                    (new MessageDialog("You can only give feedback for today's schedule", "Error")).ShowAsync().GetResults();
                    ShowProgressBar(false);
                    return;
                }
                feedControl.GetFeedback(currentSelectedSchedule.Faculty, currentSelectedSchedule.Course);
            }
        }

        private async void ProcessAvgRatingDialog()
        {
            ShowProgressBar(true);
            var avgRating = await MyILPClient.GetAvgRating(currentSelectedSchedule);
            ShowProgressBar(false);
            EnableControls(false);

            if (avgRating.Success == false)
            {
                await (new MessageDialog("No one has rated for this slot", "Error")).ShowAsync();
                ShowProgressBar(false);
                return;
            }

            feedControl.ShowAvgRating(currentSelectedSchedule.Faculty,
                currentSelectedSchedule.Course, avgRating);
        }

        private bool IsFacultyAccount(string p, out bool hasNoId)
        {
            string id = ""; 
            hasNoId = false;

            if (p.Contains('/'))
            {
                int indexofSlash = p.IndexOf('/');
                id = p.Substring(0, indexofSlash); // "962118/Milind Gour"  index = 6
            }
            else
            {
                hasNoId = true;
            }

            return id == UserInformation.EmployeeId;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //help button clicked
            string title = "Information";
            string content = "Click on a schedule to give feedback to the faculty for that slot. You can earn badges by giving feedbacks.\n\nOnce the schedule is fetched from the server, it is stored in the device for offline use. If you want to refresh the contents of the stored schedule (if the schedule is not loaded correctly or it has been changed), press Get Schedule button for 3 seconds and release.";
            (new MessageDialog(content, title)).ShowAsync().GetResults();
        }

        private void btnGetSchedule_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            e.Handled = true;
            (new MessageDialog("You have long pressed the button. This will forcefully fetch the schedule from server and update the offline copy.", "Force refresh")).ShowAsync().GetResults();

            ProcessGetScheduleUI(true);
        }
    }
}
