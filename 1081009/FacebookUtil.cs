using System;
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
using Facebook;
using Facebook.Client;
using System.Security.Cryptography;
using System.Text;
using System.Device.Location;
using System.Windows.Media;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Microsoft.Phone.Maps.Toolkit;
using System.Net.Http;
using System.Runtime.Serialization.Json;
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

    public static async void willAutoRegisterIfNeedAndAutoLogin(string AccessToken, IsolatedStorageSettings settings)
    {
        // prevent duplicated call
        if (client == null)
            client = new HttpClient();
        else
            return;

        // get fbid
         if (_facebookClient == null)
            _facebookClient = new FacebookClient(AccessToken);

        dynamic result = await _facebookClient.GetTaskAsync("me");
        string fbid = result.id;

        try
        {
            HttpResponseMessage response = await client.GetAsync(_API_URL + "auth/facebook?fbid=" + fbid);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            JObject responseJSON = JObject.Parse(responseBody);

            // not found this fbid
            if ((string)responseJSON["result"] == "user_not_found")
            {
                // will gathering register info from facebook
                dynamic me_result = await _facebookClient.GetTaskAsync("me");

                //string fbid = me_result.id;
                string username = me_result.username;
                string first_name = me_result.first_name;
                string last_name = me_result.last_name;
                string email = me_result.email;
                string password = me_result.password;

                // do register
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("fbid", fbid);
                dict.Add("username", username + "_" + fbid);
                dict.Add("first_name", first_name);
                dict.Add("last_name", last_name);
                dict.Add("email", email);
                dict.Add("password", fbid);

                HttpContent httpContent = new FormUrlEncodedContent(dict);

                HttpResponseMessage register_response = await client.PostAsync(_API_URL + "register", httpContent);
                register_response.EnsureSuccessStatusCode();
                string register_responseBody = await register_response.Content.ReadAsStringAsync();

                dynamic register_responseJSON = JObject.Parse(register_responseBody);

                // success
                if ((string)register_responseJSON["result"].result == "success")
                {
                    settings.Add("UserLogedIn", true);
                    //settings.Add("user_id", user_id);
                    settings.Add("username", username);
                    settings.Add("first_name", first_name);
                    settings.Add("last_name", last_name);
                    settings.Add("email", email);
                    settings.Save();

                    MainPage.bw.InvokeScript("showRegisterSuccess", username + "*" + first_name + "*" + last_name + "*" + email);
                }
            }
            else
            {
                // TODO : will log in by username and fbid as password
            }
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", e.Message);
        }

        /*
        if (_facebookClient == null)
            _facebookClient = new FacebookClient(AccessToken);

        dynamic result = await _facebookClient.GetTaskAsync("me");

        string fbid = result.id;
        string username = result.username;
        string first_name = result.first_name;
        string last_name = result.last_name;
        string email = result.email;

        IsolatedStorageSettings.ApplicationSettings["fbid"] = fbid;
        */
    }
}
}