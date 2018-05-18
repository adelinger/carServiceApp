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

namespace carServiceApp.My_Classes.Database
{
    class carDetailsSQL
    {
        [PrimaryKey, AutoIncrement]
        public int    id               { get; set; }
        public string markaVozila      { get; set; }
        public string tipVozila        { get; set; }
        public string modelVozila      { get; set; }
        public string godina           { get; set; }
        public string tipMotora        { get; set; }
        public string zapremninaMotora { get; set; }
        public string snagaMotora      { get; set; }
        public string uid              { get; set; }
    }
}