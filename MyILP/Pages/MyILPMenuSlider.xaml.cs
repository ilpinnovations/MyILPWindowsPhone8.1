using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MyILP.Pages
{
    public sealed partial class MyILPMenuSlider : UserControl
    {
        #region Variables
        Frame _parentFrame = null;

        int _ftc = 0; 
        const int FTC_LIMIT = 10;
        #endregion

        #region Properties
        public bool IsOpen { get; set; }
        #endregion

        public MyILPMenuSlider()
        {
            this.InitializeComponent();

            IsOpen = true;
            CloseDialog();
        }

        void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            if (IsOpen)
            {
                CloseDialog();
            }
            else
            {
                ConfirmAndExit();
            }
            e.Handled = true;
        }

        private async void ConfirmAndExit()
        {
            MessageDialog md = new MessageDialog("This will exit the app. Are you sure?", "Confirmation");
            md.Commands.Add(new UICommand("yes", null, true));
            md.Commands.Add(new UICommand("no", null, false));

            var response = await md.ShowAsync();
            bool isYes = (response != null && (bool)response.Id);
            if (isYes)
            {
                App.Current.Exit();
            }
            
        }
        public void InstallMenuSlider(Frame parentFrame)
        {
            this._parentFrame = parentFrame;
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            CloseDialog();
        }
        public void RemoveMenuSlider()
        {
            this._parentFrame = null;
            Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            //CloseDialog();
        }
        private void Rectangle_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            TestMSG();
        }

        public void TestMSG()
        {
            Windows.UI.Popups.MessageDialog md = new Windows.UI.Popups.MessageDialog("OK");
            md.ShowAsync().GetResults();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Clicked on the MenuSlider Button to open/close the slider
            //TestMSG();
            if (IsOpen)
                CloseDialog();
            else
                OpenDialog();
        }

        public void OpenDialog()
        {
            //if (!IsOpen)
            //{
            MenuSlide_In.Begin();
            IsOpen = true;
            //}
        }
        public void CloseDialog()
        {
            //if (IsOpen)
            //{
            MenuSlide_Out.Begin();
            IsOpen = false;
            //}
        }

        private void MainMenuButton_Clicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            string tagValue = (sender as Button).Tag.ToString();
            _parentFrame.BackStack.Clear();
            _ftc = 0;

            CloseDialog(); IsOpen = false;

            switch (tagValue)
            {
                case "SCHEDULE":
                    _parentFrame.Navigate(typeof(SchedulePage));
                    break;
                case "LOCATION":
                    _parentFrame.Navigate(typeof(LocationPage));
               break;
                case "BADGES":
                    _parentFrame.Navigate(typeof(BadgePage));
                    break;
                case "NOTIFICATIONS":
                    _parentFrame.Navigate(typeof(NotificationPage));
                    break;
                case "CONTACTS":
                    _parentFrame.Navigate(typeof(EContactsPage));
                    break;
                case "RESET":
                    PerformReset();
                    break;
            }
        }

        private async void PerformReset()
        {
            CloseDialog();
            string title = "Information";
            string content = "This will reset the app to its initial state. All the stored schedules as well as contact list downloaded by this app will be deleted.\nAre you sure you want to proceed?";

            MessageDialog md = new MessageDialog(content, title);
            md.Commands.Add(new UICommand("yes", null, 1));
            md.Commands.Add(new UICommand("no", null, 0));
            var result = await md.ShowAsync();
            
            if ((int)result.Id == 1)
            {
                // ok to clean
                MyILP.Code.MyILPClient.ClearLocalStorage();

                content = "The app has successfully deleted all the contents generated by it, and it will close now. Please start the app again.";
                md = new MessageDialog(content, title);
                await md.ShowAsync();
                App.Current.Exit();
            }
        }
        private void MenuHandle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Button_Click(this, e);
        }

        private async void MainMenuButton_C1icked(object sender, TappedRoutedEventArgs e)
        {
            _ftc++;
#if DEBUG
            System.Diagnostics.Debug.WriteLine("FTC = " + _ftc);
#endif
            if (_ftc == FTC_LIMIT)
            {
                _ftc = 0;
                CloseDialog();
                await (new MessageDialog(App.err_string_content, App.err_string_title)).ShowAsync();
            }
        }
    }
}
