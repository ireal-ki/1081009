using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;

namespace _1081009
{
    public partial class MenuPage : PhoneApplicationPage
    {
        public MenuPage()
        {
            InitializeComponent();

            Button UserProfileButton = (Button)this.FindName("UserProfileButton");
            TextBlock UserProfileTextBlock = (TextBlock)UserProfileButton.FindName("UserProfileTextBlock");

            string username = (string)IsolatedStorageSettings.ApplicationSettings["fb_username"];
            string first_name = (string)IsolatedStorageSettings.ApplicationSettings["fb_first_name"];
            string last_name = (string)IsolatedStorageSettings.ApplicationSettings["fb_last_name"];
            string email = (string)IsolatedStorageSettings.ApplicationSettings["fb_email"];

            UserProfileTextBlock.Text = first_name + " " + last_name + "\n" + email + "\n" + username;
        }

        /*
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
                this.NavigationService.GoBack();
            //base.OnBackKeyPress(e);

            // e.Cancel = true;
        }
         * */
    }
}