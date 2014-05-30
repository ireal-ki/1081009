using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Services;
using Windows.Devices.Geolocation;
using System.IO.IsolatedStorage;
using Facebook;
using Facebook.Client;
using System.Security.Cryptography;
using System.Text;
using System.Device.Location;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Microsoft.Phone.Maps.Toolkit;

namespace _1081009
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Url of Home page
        private string MainUri = "/Html/index.html";
        private List<string> pageNavigation = new List<string>();
        private IsolatedStorageSettings settings;
        private static WebBrowser bw = null;
        private double _accuracy = 0.0;
        private GeoCoordinate MyCoordinate = null;

        // sub content webview
        private static WebBrowser _contentWebBrowser = null;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            BackKeyPress += MainPage_BackKeyPress;
            settings = IsolatedStorageSettings.ApplicationSettings;
        }

        private void Browser_Loaded(object sender, RoutedEventArgs e)
        {
            SystemTray.ProgressIndicator = new ProgressIndicator();
            Browser.IsScriptEnabled = true;
            Browser.Navigate(new Uri(MainUri, UriKind.Relative));
            Browser.ScriptNotify += Browser_ScriptNotify;
            bw = Browser;
        }

        void appBarBtnStory_Click(object sender, EventArgs e)
        {
            HereMap.Visibility = System.Windows.Visibility.Collapsed;
            Browser.InvokeScript("onAppBarBtnStoryClick");
        }

        void appBarBtnMap_Click(object sender, EventArgs e)
        {
            HereMap.Visibility = System.Windows.Visibility.Visible;
            GetCurrentCoordinate();
            //Browser.InvokeScript("onAppBarBtnMapClick");
        }

        void appBarBtnTop10_Click(object sender, EventArgs e)
        {
            HereMap.Visibility = System.Windows.Visibility.Collapsed;
            Browser.InvokeScript("onAppBarBtnTop10Click");
        }

        private void CallMapApi()
        {
            Browser.InvokeScript("callMapApi", new string[] { MyCoordinate.Latitude.ToString(), MyCoordinate.Longitude.ToString() });
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
                int numberOfPage = pageNavigation.Count;
                string page = pageNavigation.ElementAt(numberOfPage - 2);
                pageNavigation.RemoveAt(numberOfPage-1);
                if (page == "map")
                {
                    HereMap.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    HereMap.Visibility = System.Windows.Visibility.Collapsed;
                }

                Browser.InvokeScript("onBackBtnPress", new string[] { page });
            }
        }

        void setProgressIndicator(bool isVisible)
        {
            SystemTray.ProgressIndicator.IsIndeterminate = isVisible;
            SystemTray.ProgressIndicator.IsVisible = isVisible;
        }

        void OpenIE(string url)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = new Uri(url, UriKind.Absolute);
            webBrowserTask.Show();
        }

        void FacebookLogin()
        {
            FacebookSessionClient fb = new FacebookSessionClient("303330929819566");
            fb.LoginWithApp("basic_info", "custom_state_string");
        }

        public static async void GetFacebookId()
        {
           // if (!IsolatedStorageSettings.ApplicationSettings.Contains("fbid"))
            //{
                FacebookSession session = SessionStorage.Load();
                FacebookClient client = new FacebookClient(session.AccessToken);

                dynamic result = await client.GetTaskAsync("me");
                string fbid = result.id;
                IsolatedStorageSettings.ApplicationSettings["fbid"] = fbid;
                bw.InvokeScript("fbidReturn", new string[] { fbid });
           // }
        }

        void Browser_ScriptNotify(object sender, NotifyEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Browser_ScriptNotify:" + e.Value);

            // show content
            if (e.Value.StartsWith("show_contentWebBrowser"))
            {
                // show content if exist
                if (_contentWebBrowser != null)
                    _contentWebBrowser.Visibility = System.Windows.Visibility.Visible;

                return;
            }

            // hide content
            if (e.Value.StartsWith("hide_contentWebBrowser"))
            {
                // hide content if exist
                if (_contentWebBrowser != null)
                    _contentWebBrowser.Visibility = System.Windows.Visibility.Collapsed;

                return;
            }

            // release content
            if (e.Value.StartsWith("release_contentWebBrowser"))
            {
                // hide content if exist
                if (_contentWebBrowser != null)
                    _contentWebBrowser.Visibility = System.Windows.Visibility.Collapsed;

                _contentWebBrowser = null;

                return;
            }

            if (e.Value.StartsWith("MapData"))
            {
                BindDataMap(e.Value.Split('|')[1]);
                return;
            }
            
            if (e.Value.StartsWith("fbLogin"))
            {
                FacebookLogin();
                return;
            }

            if (e.Value.StartsWith("ieOpen"))
            {
                string[] url = e.Value.Split('|');
                OpenIE(url[1]);
                return;
            }

            if (e.Value.StartsWith("responseNavigated"))
            {
                setProgressIndicator(false);
                return;
            }

            if (e.Value.StartsWith("responseNavigating"))
            {
                // hide content
                if (_contentWebBrowser != null)
                    _contentWebBrowser.Visibility = System.Windows.Visibility.Collapsed;

                setProgressIndicator(true);
                return;
            }

            if (e.Value.StartsWith("uuid"))
            {
                byte[] myDeviceID = (byte[])Microsoft.Phone.Info.DeviceExtendedProperties.GetValue("DeviceUniqueId");
                string DeviceIDAsString = Convert.ToBase64String(myDeviceID);
                Browser.InvokeScript("setUUID", new string[] { DeviceIDAsString });

                return;
            }

            if (e.Value.StartsWith("recentAdd"))
            {
                List<string> recent = null;
                if (!settings.Contains("recent"))
                {
                    recent = new List<string>();
                }else{
                    recent = (List<string>)settings["recent"];
                }
                    
                if (recent.Count >= 20)
                {
                    recent.RemoveAt(19);
                }
                else
                {
                    recent.Insert(0, e.Value);
                }

                settings["recent"] = recent;
                settings.Save();
                return;
            }

            if(e.Value.StartsWith("getRecent")){
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
                return;
            }

            if (e.Value.StartsWith("top10Loaded"))
            {
                if (!settings.Contains("WasLaunched"))
                {
                    Browser.InvokeScript("firstTimeLaunched");
                    settings.Add("WasLaunched", true);
                    settings.Save();
                }
                return;
            }

            if (e.Value.StartsWith("checkSaveLogin"))
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

            if (e.Value.StartsWith("saveUser"))
            {
                string[] str = e.Value.Split('|');

                if (settings.Contains("UserLogedIn"))
                {
                    settings["UserLogedIn"] = true;
                    settings["user_id"] = str[1];
                    settings["username"] = str[2];
                    settings["first_name"] = str[3];
                    settings["last_name"] =  str[4];
                    settings["email"] =  str[5];
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

            if (e.Value.StartsWith("logoutUser"))
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

            if (e.Value.StartsWith("AppBar"))
            {
                string[] str = e.Value.Split('|');
                if (str[1] == "true")
                    ApplicationBar.IsVisible = true;
                else
                    ApplicationBar.IsVisible = false;

                return;
            }

            // will open content webview          
            if (e.Value.StartsWith("webview"))
            {
                string[] str = e.Value.Split('|');
                ShowContentWebView(str[1]);
                return;
            }

            if (pageNavigation.Count > 0)
            {
                if (pageNavigation.Last() != e.Value)
                    pageNavigation.Add(e.Value);
            }
            else
                pageNavigation.Add(e.Value);
        }

        private void ShowContentWebView(String uriString)
        {
            // create if not exist
            if(_contentWebBrowser == null)
                _contentWebBrowser = new WebBrowser();

            _contentWebBrowser.Visibility = System.Windows.Visibility.Visible;

            // avoid top bar+bottom bar
            Thickness margin = _contentWebBrowser.Margin;
            margin.Top = 80;
            margin.Bottom = 72;
            _contentWebBrowser.Margin = margin;

            // present to view
            _contentWebBrowser.Width = LayoutRoot.Width;
            LayoutRoot.Children.Add(_contentWebBrowser);

            // nav to url
            _contentWebBrowser.Navigate(new Uri(uriString, UriKind.Absolute));
        }

        private void MyMap_Loaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "61426c47-21b0-43fb-b784-e2f06dc27b40";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "ViSgoHxp6-CYRENo80sHgA";
        }

        private async void GetCurrentCoordinate()
        {
            setProgressIndicator(true);
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracy = PositionAccuracy.High;

            try
            {
                Geoposition currentPosition = await geolocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(10));
                _accuracy = currentPosition.Coordinate.Accuracy;

                Dispatcher.BeginInvoke(() =>
                {
                    MyCoordinate = new GeoCoordinate(currentPosition.Coordinate.Latitude, currentPosition.Coordinate.Longitude);
                    CallMapApi();
                    DrawMapMarkers();
                    HereMap.SetView(MyCoordinate, 10, MapAnimationKind.Parabolic);
                });
            }
            catch (Exception)
            {
                // Couldn't get current location - location might be disabled in settings
               // MessageBox.Show(AppResources.LocationDisabledMessageBoxText, AppResources.ApplicationTitle, MessageBoxButton.OK);
            }
            setProgressIndicator(false);
        }

        private void BindDataMap(string data)
        {
            setProgressIndicator(true);
            JsonMapData dataAPiList = JsonConvert.DeserializeObject<JsonMapData>(data);
            DrawMapPushpin(dataAPiList);
            setProgressIndicator(false);
        }

        private void DrawMapPushpin(JsonMapData dataAPiList)
        {
            ObservableCollection<PushpinItems> PushpinItem = new ObservableCollection<PushpinItems>(); 
            foreach(MapData data in dataAPiList.data){
                PushpinItem.Add(new PushpinItems() { Coordinate = new GeoCoordinate(float.Parse(data.lat), float.Parse(data.lng)), Name = data.keyword, Address = data.keyword });
            }

            ObservableCollection<DependencyObject> children = MapExtensions.GetChildren(HereMap);
            var obj = children.FirstOrDefault(x => x.GetType() == typeof(MapItemsControl)) as MapItemsControl;

            try
            {
                obj.ItemsSource = PushpinItem;
            }
            catch (Exception e)
            {

            }
            
            HereMap.SetView(MyCoordinate, 16);
        }

        private void DrawMapMarkers()
        {
            HereMap.Layers.Clear();
            MapLayer mapLayer = new MapLayer();

            // Draw marker for current position
            if (MyCoordinate != null)
            {
                //DrawAccuracyRadius(mapLayer);
                DrawMapMarker(MyCoordinate, mapLayer);
            }

            // Draw markers for location(s) / destination(s)
            //DrawMapMarker(MyCoordinate, Colors.Blue, mapLayer);

            HereMap.Layers.Add(mapLayer);
        }

        private void DrawMapMarker(GeoCoordinate coordinate, MapLayer mapLayer)
        {
            // Create a MapOverlay and add marker.
            MapOverlay overlay = new MapOverlay();
            overlay.Content = new Ellipse{
                Fill = new SolidColorBrush(Colors.Blue),
                Width = 40,
                Height = 40
            };
            overlay.GeoCoordinate = new GeoCoordinate(coordinate.Latitude, coordinate.Longitude);
            overlay.PositionOrigin = new Point(0.0, 1.0);
            mapLayer.Add(overlay);
        }

        private void ZoomLevelChanged(object sender, EventArgs e)
        {
            //DrawMapMarkers();
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

                MainPage.GetFacebookId();
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

    public class PushpinItems
    {
        public GeoCoordinate Coordinate { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
    }
}
