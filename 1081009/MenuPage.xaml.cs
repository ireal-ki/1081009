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
            UserProfileButton.Tap += onTap;

            Button FavButton = (Button)this.FindName("FavButton");
            FavButton.Tap += onTap;
            FavButton.Tag = "fav";

            TextBlock UserProfileTextBlock = (TextBlock)UserProfileButton.FindName("UserProfileTextBlock");

            string username = (string)IsolatedStorageSettings.ApplicationSettings["username"];
            string first_name = (string)IsolatedStorageSettings.ApplicationSettings["first_name"];
            string last_name = (string)IsolatedStorageSettings.ApplicationSettings["last_name"];
            string email = (string)IsolatedStorageSettings.ApplicationSettings["email"];

            UserProfileTextBlock.Text = first_name + " " + last_name + "\n" + email + "\n" + username;
        }

        private void onTap(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                MainPage.model.pageCommand = ((Button)sender).Tag.ToString();

                this.NavigationService.GoBack();
            }
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