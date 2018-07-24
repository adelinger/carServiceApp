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
        public string uid               { get; set; }
        [Unique]
        public string id                { get; set; }
        public string carName           { get; set; }
        public string vrstaUsluge       { get; set; }
        public string vrstaPosla        { get; set; }
        public string opisKvara         { get; set; }
        public string datumKreiranja    { get; set; }
        public string pozeljniDatum     { get; set; }
        public bool vucnaSluzba         { get; set; }
        public bool dijelovi            { get; set; }
        public string status            { get; set; }
        public string vrijemeServisa    { get; set; }
        public string datumServisa      { get; set; }
        public string cijena            { get; set; }
        public string napomenaServisera { get; set; }
    }
}