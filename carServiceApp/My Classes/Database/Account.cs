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
        public string uid      { get; set; }
        public string name     { get; set; }
        public string lastName { get; set; }
        public string email    { get; set; }
        public string phone    { get; set; }
        public string city     { get; set; }
        public string adress   { get; set; }
    }

   
}