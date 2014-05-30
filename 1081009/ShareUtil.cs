using System;
using Microsoft.Phone.Tasks;
/*
author : katopz
*/
namespace _1081009
{
    public static class ShareUtil
    {
        public static void WillShareLink(String urlString)
        {
            // will use native wp share
            ShareLinkTask shareLinkTask = new ShareLinkTask();

            shareLinkTask.Title = "1081009";
            shareLinkTask.LinkUri = new Uri(urlString, UriKind.Absolute);
            shareLinkTask.Message = "";

            shareLinkTask.Show();
        }
    }
}