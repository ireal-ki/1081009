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
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

            // hide scrollbar
            LongListSelector MainLongListSelector = (LongListSelector)this.FindName("MainLongListSelector");
            MainLongListSelector.Loaded += (sender, e) => ((System.Windows.Controls.Primitives.ScrollBar)VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(MainLongListSelector, 0), 0), 1)).Width = 0;

            LongListSelector MainLongListSelectorRight = (LongListSelector)this.FindName("MainLongListSelectorRight");
            MainLongListSelectorRight.Loaded += (sender, e) => ((System.Windows.Controls.Primitives.ScrollBar)VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(MainLongListSelectorRight, 0), 0), 1)).Width = 0;

            // Set the data context of the LongListSelector control to the sample data
            DataContext = ViewModel;

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

        private void onTapItemRight(object sender, RoutedEventArgs e)
        {
            // TODO : like, nav to webview
            String currentTag = ((Button)sender).Tag.ToString();
            System.Diagnostics.Debug.WriteLine(" ! [FeedPage.onTapItemRight] : " + currentTag);
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

        private void onTapThumbItem(object sender, System.Windows.Input.GestureEventArgs e)
        {
            // nav to webview
            Button currentButton = (Button)sender;
            String currentTag = currentButton.Tag.ToString();
            System.Diagnostics.Debug.WriteLine(" ! [FeedPage.onTapThumbItem] : " + currentTag);


        }

        private void onTapItem(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Button currentButton = (Button)sender;
            String currentTag = currentButton.Tag.ToString();
            System.Diagnostics.Debug.WriteLine(" ! [FeedPage.onTapItem] : " + currentTag);
            Image LikeImage = (Image)currentButton.FindName("LikeImage");

            // TODO : perform like if not yet
            //LikeImage.Source = _isLike ? "/Assets/Feed/icon-like-activate.png" : "/Assets/Feed/icon-like-deactivate.png";

            BitmapImage likeIconImage = new BitmapImage(new Uri("/Assets/Feed/icon-like-activate.png", UriKind.Relative));
            LikeImage.Source = likeIconImage;
        }
    }
}