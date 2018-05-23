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
    class orders
    {
        [PrimaryKey, AutoIncrement]
        public int id               { get; set; }
        public string uid           { get; set; }
        public string carName       { get; set; }
        public string vrstaUsluge   { get; set; }
        public string vrstaPosla    { get; set; }
        public string opisKvara     { get; set; }
        public string datum         { get; set; }
        public string pozeljniDatum { get; set; }
        public bool vucnaSluzba     { get; set; }
        public bool dijelovi        { get; set; }
    }
}