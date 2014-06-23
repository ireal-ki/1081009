using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Maps.Controls;
using Windows.Devices.Geolocation;
using System.IO.IsolatedStorage;
using Facebook.Client;
using System.Security.Cryptography;
using System.Text;
using System.Device.Location;
using System.Windows.Media;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Microsoft.Phone.Maps.Toolkit;
using System.Windows.Threading;

namespace _1081009
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Url of Home page
        private string MainUri = "/Html/index.html";
        private List<string> pageNavigation = new List<string>();
        private static IsolatedStorageSettings settings;
        public static WebBrowser bw = null;
        private double _accuracy = 0.0;
        private GeoCoordinate MyCoordinate = null;

        // sub content webview
        private static WebBrowser _contentWebBrowser = null;
        private String _currentContentLink = null;

        // map
        public List<GeoCoordinate> MyCoordinates = new List<GeoCoordinate>();
        private static GeoCoordinateWatcher watcher;
        private static LocationRectangle _LocationRectangle;

        // Model ----------------------------------------------------------------------------------

        private static Model _model;

        public static Model model
        {
            get
            {
                if (_model == null)
                {
                    _model = new Model();
                }

                return _model;
            }
        }

        // Constructor
        public MainPage()
        {
            // change highlight color
            Resources.Remove("PhoneAccentColor");
            Resources.Add("PhoneAccentColor", Colors.White);
            ((SolidColorBrush)Resources["PhoneAccentBrush"]).Color = Colors.White;

            InitializeComponent();

            BackKeyPress += MainPage_BackKeyPress;
            settings = IsolatedStorageSettings.ApplicationSettings;

            this.Loaded += delegate {
                System.Diagnostics.Debug.WriteLine(" ! [MainPage.Loaded]");
                NavigationService.Navigating -= NavigationService_Navigating;
                NavigationService.Navigating += NavigationService_Navigating;

                // for test
                NavigationService.Navigate(new Uri("/FeedPage.xaml", UriKind.Relative));
            };
            this.Unloaded += delegate {
                System.Diagnostics.Debug.WriteLine(" ! [MainPage.Unloaded]");
                //NavigationService.Navigating -= NavigationService_Navigating; 
            };
        }

        void NavigationService_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(" ! [NavigationService_Navigating]");

            // Don't allow refreshing of a static page 
            //if ((e.NavigationMode == NavigationMode.Refresh) &&
            if (e.Uri.OriginalString == "/MainPage.xaml")
            {
                //e.Cancel = true;
                if (model.pageCommand != null)
                    applyCommandFromNative("navTo|" + model.pageCommand);
            }
        }

        // ----------------------------------------------------------------------------------
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            NetworkInformationUtility.GetNetworkTypeCompleted += GetNetworkTypeCompleted;
            NetworkInformationUtility.GetNetworkTypeAsync(3000); // Timeout of 3 seconds
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            NetworkInformationUtility.GetNetworkTypeCompleted -= GetNetworkTypeCompleted;
        }

        private void GetNetworkTypeCompleted(object sender, NetworkTypeEventArgs networkTypeEventArgs)
        {
            // Always dispatch on the UI thread
            Dispatcher.BeginInvoke(() =>
            {
                if (networkTypeEventArgs.HasTimeout)
                {
                    // message = "The timeout occurred";
                    no_connection_img.Visibility = Visibility.Visible;
                }
                else if (networkTypeEventArgs.HasInternet)
                {
                    //message = "The Internet connection type is: " + networkTypeEventArgs.Type.ToString();
                    no_connection_img.Visibility = Visibility.Collapsed;
                }
                else
                {
                    //message = "There is no Internet connection";
                    no_connection_img.Visibility = Visibility.Visible;
                }

                System.Diagnostics.Debug.WriteLine(" ! has internet : " + (no_connection_img.Visibility == Visibility.Collapsed));
            });
        }

        // ----------------------------------------------------------------------------------

        private void Browser_Loaded(object sender, RoutedEventArgs e)
        {
            if (bw != null)
                return;

            Browser.IsScriptEnabled = true;
            Browser.Navigate(new Uri(MainUri, UriKind.Relative));
            Browser.ScriptNotify += Browser_ScriptNotify;
            bw = Browser;
        }

        void appBarBtnStory_Click(object sender, EventArgs e)
        {
            MyMap.Visibility = System.Windows.Visibility.Collapsed;
            Browser.InvokeScript("onAppBarBtnStoryClick");
        }

        void appBarBtnMap_Click(object sender, EventArgs e)
        {
            MyMap.Visibility = System.Windows.Visibility.Visible;

            GetCurrentCoordinate();
            //Browser.InvokeScript("onAppBarBtnMapClick");

            // hide AppBar
            ApplicationBar.IsVisible = false;

            // for nav refference to current page later when hit back button
            addPageNav("map");
        }

        void appBarBtnTop10_Click(object sender, EventArgs e)
        {
            MyMap.Visibility = System.Windows.Visibility.Collapsed;
            Browser.InvokeScript("onAppBarBtnTop10Click");
        }

        void MainPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var isGoback = false;
            if (pageNavigation != null)
            {
                if (pageNavigation.Count > 1)
                    isGoback = true;
                else
                    isGoback = false;
            }
            else
                isGoback = false;

            if (isGoback)
            {
                e.Cancel = true;

                goback();
            }
        }

        private void goback()
        {
            // hide loading
            setProgressIndicator(false);

            int numberOfPage = pageNavigation.Count;

            string currentPage = pageNavigation.ElementAt(numberOfPage - 1);
            string targetPage = pageNavigation.ElementAt(numberOfPage - 2);

            pageNavigation.RemoveAt(numberOfPage - 1);

            System.Diagnostics.Debug.WriteLine(" * [back] currentPage:" + currentPage);
            System.Diagnostics.Debug.WriteLine(" * [back] targetPage:" + targetPage);

            if (targetPage == "map")
            {
                MyMap.Visibility = System.Windows.Visibility.Visible;

                // hide AppBar
                ApplicationBar.IsVisible = false;
            }
            else
            {
                MyMap.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (targetPage == "feed")
            {
                Browser.InvokeScript("restoreFeed");
            }
            else
            {
                Browser.InvokeScript("navTo", new string[] { targetPage });
            }
        }

        void whenHideOverlay()
        {
            // also pop nav
            if (pageNavigation.Count>1)
                pageNavigation.RemoveAt(pageNavigation.Count - 1);
        }

        void setProgressIndicator(bool isVisible)
        {
            if (isVisible)
                CustomProgressIndicator.Visibility = System.Windows.Visibility.Visible;
            else
                CustomProgressIndicator.Visibility = System.Windows.Visibility.Collapsed;
        }

        void OpenIE(string url)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = new Uri(url, UriKind.Absolute);
            webBrowserTask.Show();
        }

        private void Browser_ScriptNotify(object sender, NotifyEventArgs e)
        {
            applyCommand(e.Value);
        }

        void applyCommandFromNative(string pageCommand)
        {
            // fake command from native
            if (pageCommand.StartsWith("navTo|"))
            {
                string targetPage = pageCommand.Split('|')[1];
                Browser.InvokeScript("navTo", new string[] { targetPage });
                return;
            }
        }

        void applyCommand(string pageCommand)
        {
            if (pageCommand.StartsWith("log|"))
            {
                // just log 
                System.Diagnostics.Debug.WriteLine(" ! [Log] : " + pageCommand.Split('|')[1]);
               return;
            }
            else  if (pageCommand.StartsWith("showDialog|"))
            {
                string[] msgs = pageCommand.Split('|');
                MessageBox.Show(msgs[1]);
                return;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(" ! [applyCommand] : " + pageCommand);
            }

            //
            if (pageCommand.StartsWith("hide-overlay"))
            {
                whenHideOverlay();
                return;
            }

            // show wp share
            if (pageCommand.StartsWith("share_content"))
            {
                ShareUtil.WillShareLink(_currentContentLink);

                return;
            }

            // show content
            if (pageCommand.StartsWith("show_contentWebBrowser"))
            {
                // show content if exist
                if (_contentWebBrowser != null)
                    _contentWebBrowser.Visibility = System.Windows.Visibility.Visible;

                return;
            }

            // hide content
            if (pageCommand.StartsWith("hide_contentWebBrowser"))
            {
                // hide content if exist
                if (_contentWebBrowser != null)
                    _contentWebBrowser.Visibility = System.Windows.Visibility.Collapsed;

                return;
            }

            // release content
            if (pageCommand.StartsWith("release_contentWebBrowser"))
            {
                // hide content if exist
                if (_contentWebBrowser != null)
                    _contentWebBrowser.Visibility = System.Windows.Visibility.Collapsed;

                _contentWebBrowser = null;

                return;
            }

            if (pageCommand.StartsWith("MapData"))
            {
                BindDataMap(pageCommand.Split('|')[1]);
                return;
            }

            if (pageCommand.StartsWith("registerWithFacebook"))
            {
                FacebookUtil.willRegisterWithFacebookAndLogin(pageCommand.Split('|')[1]);
                return;
            }

            if (pageCommand.StartsWith("fbLogin"))
            {
                FacebookUtil.LoginWithApp();
                return;
            }

            // TODO : replace with native
            if (pageCommand.StartsWith("userMenu"))
            {
                NavigationService.Navigate(new Uri("/MenuPage.xaml", UriKind.Relative));
                addPageNav(pageCommand);
                return;
            }

            if (pageCommand.StartsWith("ieOpen"))
            {
                string[] url = pageCommand.Split('|');
                OpenIE(url[1]);
                return;
            }

            if (pageCommand.StartsWith("responseNavigated"))
            {
                setProgressIndicator(false);
                return;
            }

            if (pageCommand.StartsWith("responseNavigating"))
            {
                setProgressIndicator(true);
                return;
            }

            if (pageCommand.StartsWith("uuid"))
            {
                byte[] myDeviceID = (byte[])Microsoft.Phone.Info.DeviceExtendedProperties.GetValue("DeviceUniqueId");
                string DeviceIDAsString = Convert.ToBase64String(myDeviceID);
                Browser.InvokeScript("setUUID", new string[] { DeviceIDAsString });

                return;
            }

            if (pageCommand.StartsWith("recentAdd"))
            {
                List<string> recent = null;
                if (!settings.Contains("recent"))
                {
                    recent = new List<string>();
                }
                else
                {
                    recent = (List<string>)settings["recent"];
                }

                if (recent.Count >= 20)
                {
                    recent.RemoveAt(19);
                }
                else
                {
                    recent.Insert(0, pageCommand);
                }

                settings["recent"] = recent;
                settings.Save();
                return;
            }

            if (pageCommand.StartsWith("getRecent"))
            {
                string recentsList = "";
                List<string> recent = (List<string>)settings["recent"];
                for (int i = 0; i < recent.Count; i++)
                {
                    if (i != 0)
                    {
                        recentsList += "^";
                    }
                    recentsList += recent[i];
                }
                Browser.InvokeScript("getRecentReturn", new string[] { recentsList });

                // hide loading
                setProgressIndicator(false);

                // hide map if need
                if (pageCommand != "map")
                    MyMap.Visibility = System.Windows.Visibility.Collapsed;

                addPageNav(pageCommand);
                return;
            }

            if (pageCommand.StartsWith("top10Loaded"))
            {
                if (!settings.Contains("WasLaunched"))
                {
                    Browser.InvokeScript("firstTimeLaunched");
                    settings.Add("WasLaunched", true);
                    settings.Save();
                }
                return;
            }

            if (pageCommand.StartsWith("checkSaveLogin"))
            {
                if (settings.Contains("UserLogedIn"))
                {
                    bool userLogedIn = (bool)settings["UserLogedIn"];
                    if (userLogedIn)
                    {
                        string user_id = (string)settings["user_id"];
                        string username = (string)settings["username"];
                        string first_name = (string)settings["first_name"];
                        string last_name = (string)settings["last_name"];
                        string email = (string)settings["email"];
                        Browser.InvokeScript("SetUser", new string[] { user_id, username, first_name, last_name, email });
                    }
                }
                return;
            }

            if (pageCommand.StartsWith("saveUser"))
            {
                string[] str = pageCommand.Split('|');

                if (settings.Contains("UserLogedIn"))
                {
                    settings["UserLogedIn"] = true;
                    settings["user_id"] = str[1];
                    settings["username"] = str[2];
                    settings["first_name"] = str[3];
                    settings["last_name"] = str[4];
                    settings["email"] = str[5];
                }
                else
                {
                    settings.Add("UserLogedIn", true);
                    settings.Add("user_id", str[1]);
                    settings.Add("username", str[2]);
                    settings.Add("first_name", str[3]);
                    settings.Add("last_name", str[4]);
                    settings.Add("email", str[5]);
                }
                settings.Save();
                return;
            }

            if (pageCommand.StartsWith("logoutUser"))
            {
                if (settings.Contains("UserLogedIn"))
                {
                    settings["UserLogedIn"] = false;
                    settings["user_id"] = "";
                    settings["username"] = "";
                    settings["first_name"] = "";
                    settings["last_name"] = "";
                    settings["email"] = "";
                    settings["fbid"] = "";
                    settings.Remove("fbid");
                }
                else
                {
                    settings.Add("UserLogedIn", false);
                    settings.Add("user_id", "");
                    settings.Add("username", "");
                    settings.Add("first_name", "");
                    settings.Add("last_name", "");
                    settings.Add("email", "");
                    settings.Add("fbid", "");
                    settings.Remove("fbid");
                }

                settings.Save();
                return;
            }

            if (pageCommand.StartsWith("AppBar"))
            {
                string[] str = pageCommand.Split('|');
                if (str[1] == "true")
                    ApplicationBar.IsVisible = true;
                else
                    ApplicationBar.IsVisible = false;

                return;
            }

            // will open content webview          
            if (pageCommand.StartsWith("webview"))
            {
                string[] str = pageCommand.Split('|');
                ShowContentWebView(str[1]);

                // hide map if need
                if (pageCommand != "map")
                    MyMap.Visibility = System.Windows.Visibility.Collapsed;

                addPageNav("webview");
                return;
            }

            // hide loading
            setProgressIndicator(false);

            // hide map if need
            if (pageCommand != "map")
                MyMap.Visibility = System.Windows.Visibility.Collapsed;

            // default case will count as page nav
            addPageNav(pageCommand);
        }

        private void addPageNav(String pageString)
        {
            System.Diagnostics.Debug.WriteLine(" * addPageNav : " + pageString);

            if (pageNavigation.Count > 0)
            {
                if (pageNavigation.Last() != pageString)
                    pageNavigation.Add(pageString);
            }
            else
            {
                pageNavigation.Add(pageString);
            }
        }

        private void ShowContentWebView(String uriString)
        {
            // create if not exist
            if (_contentWebBrowser == null)
            {
                _contentWebBrowser = new WebBrowser();

                // avoid top bar+bottom bar
                Thickness margin = _contentWebBrowser.Margin;
                margin.Top = 80;
                margin.Bottom = 72;
                _contentWebBrowser.Margin = margin;

                // present to view
                _contentWebBrowser.Width = LayoutRoot.Width;
                LayoutRoot.Children.Add(_contentWebBrowser);

                // will open external link via IE
                _contentWebBrowser.Navigating += new EventHandler<NavigatingEventArgs>(_contentWebBrowser_Navigating);
            }

            _contentWebBrowser.Visibility = System.Windows.Visibility.Visible;

            // nav to url
            if (_currentContentLink != uriString)
            {
                // for later use
                _currentContentLink = uriString;

                _contentWebBrowser.Navigate(new Uri(uriString, UriKind.Absolute));
            }
        }

        private void _contentWebBrowser_Navigating(object sender, NavigatingEventArgs e)
        {
            String uriString = e.Uri.AbsoluteUri;

            if (uriString.StartsWith("http://") && !uriString.Equals(_currentContentLink))
            {
                e.Cancel = true;
                OpenIE(uriString);
            }
        }

        private void MyMap_Loaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "61426c47-21b0-43fb-b784-e2f06dc27b40";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "ViSgoHxp6-CYRENo80sHgA";
        }

        private async void GetCurrentCoordinate()
        {
            System.Diagnostics.Debug.WriteLine(" ! GetCurrentCoordinate");

            setProgressIndicator(true);

            if (watcher == null)
            {
                watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
                watcher.MovementThreshold = 20;
                watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
                watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
            }

            watcher.Start();
        }

        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    MessageBox.Show("Location Service is not enabled on the device");
                    setProgressIndicator(false);
                    break;

                case GeoPositionStatus.NoData:
                    MessageBox.Show(" The Location Service is working, but it cannot get location data.");
                    setProgressIndicator(false);
                    break;
            }
        }

        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            if (e.Position.Location.IsUnknown)
            {
                MessageBox.Show("Please wait while your prosition is determined....");
                return;
            }

            // enough
            watcher.Stop();

            MyCoordinate = watcher.Position.Location;

            System.Diagnostics.Debug.WriteLine(" ! GetCurrentCoordinate.SetView");

            // TODO : check if not far from previous location will use old data, or add button to relocate
            if(_LocationRectangle != null)
                MyMap.SetView(_LocationRectangle);
            else
                MyMap.SetView(MyCoordinate, 8, MapAnimationKind.Parabolic);

            Pushpin MyPushpin = (Pushpin)this.FindName("MyPushpin");
            MyPushpin.GeoCoordinate = MyCoordinate;
            
            // get near by data
            Browser.InvokeScript("callMapApi", new string[] { MyCoordinate.Latitude.ToString(), MyCoordinate.Longitude.ToString() });
        }

        private void BindDataMap(string data)
        {
            System.Diagnostics.Debug.WriteLine(" ! BindDataMap");

            setProgressIndicator(true);
            JsonMapData dataAPiList = JsonConvert.DeserializeObject<JsonMapData>(data);
            DrawMapPushpin(dataAPiList);
            setProgressIndicator(false);
        }

        private void DrawMapPushpin(JsonMapData dataAPiList)
        {
            System.Diagnostics.Debug.WriteLine(" ! DrawMapPushpin");

            ObservableCollection<PushpinModel> Pushpins = new ObservableCollection<PushpinModel>();

            MyCoordinates.Clear();
            foreach (MapData data in dataAPiList.data)
            {
                GeoCoordinate _GeoCoordinate = new GeoCoordinate(float.Parse(data.lat), float.Parse(data.lng));
                Pushpins.Add(new PushpinModel()
                {
                    Coordinate = _GeoCoordinate,
                    ID = data.id,
                    Name = data.keyword,
                    Address = data.keyword,
                    ImageURI = data.image_url
                });

                MyCoordinates.Add(_GeoCoordinate);
            }

            ObservableCollection<DependencyObject> children = MapExtensions.GetChildren(MyMap);
            var obj = children.FirstOrDefault(x => x.GetType() == typeof(MapItemsControl)) as MapItemsControl;

            try
            {
                obj.ItemsSource = Pushpins;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(" ! Exception : " + e);
            }

            MyMap.Center = MyCoordinates[MyCoordinates.Count - 1];

            Dispatcher.BeginInvoke(() =>
            {
                _LocationRectangle = LocationRectangle.CreateBoundingRectangle(MyCoordinates);
                MyMap.SetView(_LocationRectangle);
            });

            MyMap.SetView(MyCoordinates[MyCoordinates.Count - 1], 10, MapAnimationKind.Linear);
        }

        private void ZoomLevelChanged(object sender, EventArgs e)
        {
            // do something
        }

        private void Pushpin_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Pushpin pushpin = sender as Pushpin;

            System.Diagnostics.Debug.WriteLine(" ! Pushpin_Tap : " + pushpin.Content);

            // not me
            if (String.Equals((string)pushpin.Content, "My Location"))
                return;

            string amphoe_id = (string)pushpin.Tag;

            // search by content
            Browser.InvokeScript("searchFromMap", new string[] { amphoe_id, (string)pushpin.Content });
        }

        // Navigates back in the web browser's navigation stack, not the applications.
        private void BackApplicationBar_Click(object sender, EventArgs e)
        {
            Browser.GoBack();
        }

        // Navigates forward in the web browser's navigation stack, not the applications.
        private void ForwardApplicationBar_Click(object sender, EventArgs e)
        {
            Browser.GoForward();
        }

        // Navigates to the initial "home" page.
        private void HomeMenuItem_Click(object sender, EventArgs e)
        {
            Browser.Navigate(new Uri(MainUri, UriKind.Relative));
        }

        // Handle navigation failures.
        private void Browser_NavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            MessageBox.Show("Navigation to this page failed, check your internet connection");
        }

        private void Browser_Navigated(object sender, NavigationEventArgs e)
        {
            //setProgressIndicator(false);
        }
    }

    public class SessionStorage
    {
        /// <summary>
        /// Key used to store access token in app settings
        /// </summary>
        private const string AccessTokenSettingsKeyName = "fb_access_token";

        /// <summary>
        /// Key used to store access token expiry in app settings
        /// </summary>
        private const string AccessTokenExpirySettingsKeyName = "fb_access_token_expiry";

        /// <summary>
        /// Key used to state in app settings
        /// </summary>
        private const string StateSettingsKeyName = "fb_login_state";

        /// <summary>
        /// Tries to retrieve a session
        /// </summary>
        /// <returns>
        /// A valid login response with access token and expiry, or null (including if token already expired)
        /// </returns>
        public static FacebookSession Load()
        {
            // read access token
            string accessTokenValue = LoadEncryptedSettingValue(AccessTokenSettingsKeyName);

            // read expiry
            DateTime expiryValue = DateTime.MinValue;
            string expiryTicks = LoadEncryptedSettingValue(AccessTokenExpirySettingsKeyName);
            if (!string.IsNullOrWhiteSpace(expiryTicks))
            {
                long expiryTicksValue = 0;
                if (long.TryParse(expiryTicks, out expiryTicksValue))
                {
                    expiryValue = new DateTime(expiryTicksValue);
                }
            }

            // read state
            string stateValue = LoadEncryptedSettingValue(StateSettingsKeyName);

            // return true only if both values retrieved and token not stale
            if (!string.IsNullOrWhiteSpace(accessTokenValue) && expiryValue > DateTime.UtcNow)
            {
                return new FacebookSession()
                {
                    AccessToken = accessTokenValue,
                    Expires = expiryValue,
                    State = stateValue
                };
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Saves an access token an access token and its expiry
        /// </summary>
        /// <param name="session">A valid login response with access token and expiry</param>
        public static void Save(FacebookSession session)
        {
            SaveEncryptedSettingValue(AccessTokenSettingsKeyName, session.AccessToken);
            SaveEncryptedSettingValue(AccessTokenExpirySettingsKeyName, session.Expires.Ticks.ToString());
            SaveEncryptedSettingValue(StateSettingsKeyName, session.State);
        }

        /// <summary>
        /// Removes saved values for access token and expiry
        /// </summary>
        public static void Remove()
        {
            RemoveEncryptedSettingValue(AccessTokenSettingsKeyName);
            RemoveEncryptedSettingValue(AccessTokenExpirySettingsKeyName);
            RemoveEncryptedSettingValue(StateSettingsKeyName);
        }

        /// <summary>
        /// Removes an encrypted setting value
        /// </summary>
        /// <param name="key">Key to remove</param>
        private static void RemoveEncryptedSettingValue(string key)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
            {
                IsolatedStorageSettings.ApplicationSettings.Remove(key);
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        /// <summary>
        /// Loads an encrypted setting value for a given key
        /// </summary>
        /// <param name="key">The key to load</param>
        /// <returns>
        /// The value of the key
        /// </returns>
        /// <exception cref="KeyNotFoundException">The given key was not found</exception>
        private static string LoadEncryptedSettingValue(string key)
        {
            string value = null;
            if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
            {
                var protectedBytes = IsolatedStorageSettings.ApplicationSettings[key] as byte[];
                if (protectedBytes != null)
                {
                    byte[] valueBytes = ProtectedData.Unprotect(protectedBytes, null);
                    value = Encoding.UTF8.GetString(valueBytes, 0, valueBytes.Length);
                }
            }

            return value;
        }

        /// <summary>
        /// Saves a setting value against a given key, encrypted
        /// </summary>
        /// <param name="key">The key to save against</param>
        /// <param name="value">The value to save against</param>
        /// <exception cref="System.ArgumentOutOfRangeException">The key or value provided is unexpected</exception>
        private static void SaveEncryptedSettingValue(string key, string value)
        {
            if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
            {
                byte[] valueBytes = Encoding.UTF8.GetBytes(value);

                // Encrypt the value by using the Protect() method.
                byte[] protectedBytes = ProtectedData.Protect(valueBytes, null);
                if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
                {
                    IsolatedStorageSettings.ApplicationSettings[key] = protectedBytes;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings.Add(key, protectedBytes);
                }

                IsolatedStorageSettings.ApplicationSettings.Save();
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }

    public class FacebookUriMapper : UriMapperBase
    {
        private bool facebookLoginHandled;

        public override Uri MapUri(Uri uri)
        {
            System.Diagnostics.Debug.WriteLine(" ! MapUri : " + uri.ToString());

            if (AppAuthenticationHelper.IsFacebookLoginResponse(uri))
            {
                FacebookSession session = new FacebookSession();
                try
                {
                    session.ParseQueryString(HttpUtility.UrlDecode(uri.ToString()));

                    // Handle success case
                    // do something with the custom state parameter
                    if (session.State != "custom_state_string")
                    {
                        MessageBox.Show("Unexpected state: " + session.State);
                    }
                    else
                    {
                        // save the token and continue (token is retrieved and used when the app
                        // is launched)
                        SessionStorage.Save(session);
                    }
                }
                catch (Facebook.FacebookOAuthException exc)
                {
                    if (!this.facebookLoginHandled)
                    {
                        // Handle error case
                        MessageBox.Show("Not signed in: " + exc.Message);
                        this.facebookLoginHandled = true;
                    }
                }

                FacebookUtil.willLoginWithFacebookID(session.AccessToken);

                return new Uri("/MainPage.xaml", UriKind.Relative);
            }
            // by default, navigate to the requested uri
            return uri;
        }
    }

    public class JsonMapData
    {
        public List<MapData> data { get; set; }
    }

    public class MapData
    {
        public string id { get; set; }
        public string title { get; set; }
        public string keyword { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public int amount { get; set; }
        public string image_url { get; set; }
    }

    public class PushpinModel
    {
        public GeoCoordinate Coordinate { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string ImageURI { get; set; }
        public string ID { get; set; }
    }
}
