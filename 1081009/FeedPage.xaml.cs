using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using _1081009.ViewModels;

namespace _1081009
{
    public partial class FeedPage : PhoneApplicationPage
    {
        private static MainViewModel viewModel = null;

        /// <summary>
        /// A static ViewModel used by the views to bind against.
        /// </summary>
        /// <returns>The MainViewModel object.</returns>
        public static MainViewModel ViewModel
        {
            get
            {
                // Delay creation of the view model until necessary
                if (viewModel == null)
                    viewModel = new MainViewModel();

                return viewModel;
            }
        }

        public FeedPage()
        {
            InitializeComponent();

            // Ensure that application state is restored appropriately
            if (!ViewModel.IsDataLoaded)
            {
                ViewModel.LoadData();
            }
        }

        /*********************************************************
        * Handle selection changed on LongListSelector
        *********************************************************/
        private void MainLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected item is null (no selection) do nothing
            if (MainLongListSelector.SelectedItem == null)
                return;

            // Navigate to the new page
            //NavigationService.Navigate(new Uri("/DetailsPage.xaml?selectedItem=" + (MainLongListSelector.SelectedItem as ItemViewModel).ID, UriKind.Relative));

            // Reset selected item to null (no selection)
            MainLongListSelector.SelectedItem = null;
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
    }
}