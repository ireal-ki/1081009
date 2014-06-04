using System;
using System.Collections.Generic;
using System.Windows;
using System.IO.IsolatedStorage;
using Facebook;
using Facebook.Client;
using System.Net.Http;
using Newtonsoft.Json.Linq;
/*
author : katopz
*/
namespace _1081009
{
    /// <summary>
    /// Summary description for FacebookUtil
    /// </summary>
    public class FacebookUtil
    {
        private const string _APP_ID = "321684751264693";
        private const string _API_URL = "http://1081009.tourismthailand.org/api/";
        private static FacebookSessionClient _facebookSessionClient;
        private static FacebookClient _facebookClient;
        private static HttpClient client;

        public static void LoginWithApp()
        {
            if (_facebookSessionClient == null)
                _facebookSessionClient = new FacebookSessionClient(_APP_ID);

            _facebookSessionClient.LoginWithApp("basic_info,publish_actions,email", "custom_state_string");
        }

        public static async void willLoginWithFacebookID(string AccessToken)
        {
            // get fbid
            if (_facebookClient == null)
                _facebookClient = new FacebookClient(AccessToken);
            else
                return;
            
            dynamic me_result = await _facebookClient.GetTaskAsync("me");
            string fbid = me_result.id;

            // for later use
            IsolatedStorageSettings.ApplicationSettings["fbid"] = fbid;
            IsolatedStorageSettings.ApplicationSettings["username"] = me_result.username;
            IsolatedStorageSettings.ApplicationSettings["first_name"] = me_result.first_name;
            IsolatedStorageSettings.ApplicationSettings["last_name"] = me_result.last_name;
            IsolatedStorageSettings.ApplicationSettings["email"] = me_result.email;
            IsolatedStorageSettings.ApplicationSettings.Save();

            // call js
            MainPage.bw.InvokeScript("fbidReturn", new string[] { fbid });
        }

        public static async void willRegisterWithFacebookAndLogin(string fbid)
        {
            // prevent duplicated call
            if (client == null)
                client = new HttpClient();
            else
                return;

            try
            {
                // string fbid = me_result.id;
                string username = (string)IsolatedStorageSettings.ApplicationSettings["username"];
                string first_name = (string)IsolatedStorageSettings.ApplicationSettings["first_name"];
                string last_name = (string)IsolatedStorageSettings.ApplicationSettings["last_name"];
                string email = (string)IsolatedStorageSettings.ApplicationSettings["email"];

                // do register : TODO send to js registering view and deprecated this?
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("fbid", fbid);
                dict.Add("username", username);
                dict.Add("first_name", first_name);
                dict.Add("last_name", last_name);
                dict.Add("email", email);
                dict.Add("password", fbid);

                HttpContent httpContent = new FormUrlEncodedContent(dict);

                HttpResponseMessage register_response = await client.PostAsync(_API_URL + "register", httpContent);
                register_response.EnsureSuccessStatusCode();
                string register_responseBody = await register_response.Content.ReadAsStringAsync();

                dynamic register_responseJSON = JObject.Parse(register_responseBody);

                switch ((string)register_responseJSON["result"])
                {
                    case "success":
                        {
                            MainPage.bw.InvokeScript("fbidReturn", new string[] { fbid });
                            break;
                        }
                    case "user_existing":
                        {
                            MessageBox.Show("Sorry, User already exist");
                        }
                        break;
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }
    }
}