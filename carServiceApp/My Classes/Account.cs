using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Java.IO;
using Java.Lang;
using Java;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Annotation;
using Android.Views;
using Android.Widget;
using Firebase;
using Firebase.Database;
using Firebase.Xamarin;
using Newtonsoft.Json;

namespace carServiceApp.My_Classes
{
   
   public class Account
    {
        [JsonProperty ("uid"), JsonIgnore]
        public string uid      { get; set; }
        [JsonProperty("name")]
        public string name     { get; set; }
        [JsonProperty("lastName")]
        public string lastName { get; set; }
        [JsonProperty("email")]
        public string email    { get; set; }
        [JsonProperty("phone")]
        public string phone    { get; set; }
        [JsonProperty("rememberMe")]
        public bool rememberMe { get; set; }

        public Account()
        {

        }

    }
 
}