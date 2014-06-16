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

            string username = (string)IsolatedStorageSettings.ApplicationSettings["username"];
            string first_name = (string)IsolatedStorageSettings.ApplicationSettings["first_name"];
            string last_name = (string)IsolatedStorageSettings.ApplicationSettings["last_name"];
            string email = (string)IsolatedStorageSettings.ApplicationSettings["email"];

            UserProfileTextBlock.Text = first_name + " " + last_name + "\n" + email + "\n" + username;

            this.Unloaded += onUnloaded;
        }

        private void onUnloaded(object sender, RoutedEventArgs e)
        {
            // TODO : move to header
            Button HomeButton = (Button)this.FindName("HomeButton");
            HomeButton.Tap -= onTap;

            Button SearchButton = (Button)this.FindName("SearchButton");
            SearchButton.Tap -= onTap;

            // buttons
            Button UserProfileButton = (Button)this.FindName("UserProfileButton");
            UserProfileButton.Tap -= onTap;

            Button FavButton = (Button)this.FindName("FavButton");
            FavButton.Tap -= onTap;

            Button MyStoryButton = (Button)this.FindName("MyStoryButton");
            MyStoryButton.Tap -= onTap;

            Button AboutButton = (Button)this.FindName("AboutButton");
            AboutButton.Tap -= onTap;

            Button AddArticleButton = (Button)this.FindName("AddArticleButton");
            AddArticleButton.Tap -= onTap;

            //getHomeFeed
        }

        private void onTap(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                // inject page command for MainPage excution after go back
                MainPage.model.pageCommand = ((Button)sender).Tag.ToString();

                // do go back
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