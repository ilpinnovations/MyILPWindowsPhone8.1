using MyILP.Code;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MyILP
{
    public delegate void FeedbackHandler(FeedbackObject feedbackObj);
    public sealed partial class FeedbackControl : UserControl
    {
        enum Mode { GiveFeedback, ShowRating }
        private bool _isOpen = false;
        public event FeedbackHandler FeedbackReceived;
        public event FeedbackHandler FeedbackCancelled;

        private void OnFeedbackCancelled(FeedbackObject o)
        {
            if (FeedbackCancelled != null)
                FeedbackCancelled(o);
        }
        private void OnFeedbackReceived(FeedbackObject o)
        {
            if (FeedbackReceived != null)
                FeedbackReceived(o);
        }
        public bool IsOpen
        {
            get { return _isOpen; }
            set
            {
                if (_isOpen != value)
                {
                    if (_isOpen)
                    {
                        FeedbackControl_Exit.Begin();
                        _isOpen = value;
                    }
                    else
                    {
                        ResetFields();
                        FeedbackControl_Entry.Begin();
                        _isOpen = value;
                    }
                }
            }
        }
        private void ResetFields()
        {
            tbComments.Text = string.Empty;
            rating.Value = 0;
        }
        public FeedbackControl()
        {
            this.InitializeComponent();
            IsOpen = false;
        }
        public void GetFeedback(string facultyName, string courseName)
        {
            if (!IsOpen)
            {
                SetDialogMode(Mode.GiveFeedback);
                tbCourseName.Text = courseName;
                tbFacultyName.Text = facultyName;
                IsOpen = true;
            }
        }
        public void ShowAvgRating(string facultyName, string courseName, AvgRatingObject avgRateObj)
        {
            if (!IsOpen)
            {
                SetDialogMode(Mode.ShowRating);
                tbCourseName.Text = courseName;
                tbFacultyName.Text = facultyName;

                double avgRounded = Math.Round(avgRateObj.AvgRating, 2);
                tbRating.Text = avgRounded.ToString();
                double offsetValue = avgRounded / 5;
                tbFeedbacks.Text = avgRateObj.TotalComments.ToString();
                
                IsOpen = true;
                rating.Offset = offsetValue;

            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (rating.Value == 0)
            {
                await (new MessageDialog("Rating cannot be zero", "Error")).ShowAsync();
                return;
            }

            //submit clicked
            FeedbackObject feedObject = new FeedbackObject(tbFacultyName.Text,
                tbCourseName.Text, rating.Value, tbComments.Text);

            IsOpen = false;
            OnFeedbackReceived(feedObject);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //cancel clicked
            IsOpen = false;
            FeedbackObject feedObject = new FeedbackObject(tbFacultyName.Text, tbCourseName.Text, 0, string.Empty);
            OnFeedbackCancelled(feedObject);
        }
        private void SetDialogMode(Mode mode)
        {
            switch (mode)
            {
                case Mode.GiveFeedback:
                    spComments.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    spAvgRating.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    bSubmit.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    rating.IsEnabled = true;
                    bCancel.Content = "Cancel";
                    break;
                case Mode.ShowRating:
                    spComments.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    spAvgRating.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    bSubmit.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    rating.IsEnabled = false;
                    bCancel.Content = "Close";
                    break;
                default:
                    break;
            }
        }
    }
}
