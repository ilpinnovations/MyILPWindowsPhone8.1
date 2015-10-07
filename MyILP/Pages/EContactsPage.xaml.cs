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
using Windows.ApplicationModel.Chat;
using System.Net.Http;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace MyILP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EContactsPage : Page
    {
        public EContactsPage()
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

            ProcessGetEContactsUI();
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            menu.RemoveMenuSlider();
        }

        private void ShowProgressBar(bool show)
        {
            progressBar.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }
        private async void ProcessGetEContactsUI()
        {
            ShowProgressBar(true);

            try
            {
                EContactModel contacts = await MyILP.Code.MyILPClient.GetEContacts();
                listEContacts.DataContext = contacts.EContactItems;
            }
            catch (HttpRequestException exception)
            {
                (new MessageDialog(exception.Message, "Error")).ShowAsync().GetResults();
            }
            finally
            {
                ShowProgressBar(false);
            }

        }

        private void listEContacts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Item clicked, provide user with the options to call / msg the selected number
            ListView cb = sender as ListView;
            if (cb.SelectedIndex == -1) return;

            EContactItem selectedItem = cb.SelectedItem as EContactItem;
            cb.SelectedIndex = -1;
            ProcessEContactClick(selectedItem);
        }

        private async void ProcessEContactClick(EContactItem selectedItem)
        {
            string content = string.Format("Call or message {0}?\nPress back to cancel.", selectedItem.Name);
            string title = "Information";

            string[] cmdString = new string[] { "call", "message" };
            MessageDialog md = new MessageDialog(content, title);
            md.Commands.Add(new UICommand(cmdString[0]));
            md.Commands.Add(new UICommand(cmdString[1]));

            IUICommand result = await md.ShowAsync();
            if (result == null) return;

            string resultString = result.Label;
            if (resultString == cmdString[0])
            {
                // Process call
                Windows.ApplicationModel.Calls.PhoneCallManager.ShowPhoneCallUI(selectedItem.Contact, selectedItem.Name);
            }
            else if (resultString == cmdString[1])
            {
                // Process message
                Windows.ApplicationModel.Chat.ChatMessage chatMsg = new Windows.ApplicationModel.Chat.ChatMessage();
                chatMsg.Recipients.Add(selectedItem.Contact);
                chatMsg.Body = "";
                await ChatMessageManager.ShowComposeSmsMessageAsync(chatMsg);
            }
        }
    }
}
