using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;

namespace carServiceApp.My_Classes
{
    class User
    {
        public bool rememberMe { get; set; }
        [PrimaryKey]
        public string uid      { get; set; }
        public string name     { get; set; }
        public string lastName { get; set; }
        [Unique]
        public string email    { get; set; }
        public string phone    { get; set; }
        public string city     { get; set; }
        public string adress  { get; set; }
    }
}