using System;
using System.Collections.Generic;
using System.IO;
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
    class connection
    {
        public string       dbPath { get; set; }
        public SQLiteConnection db { get; set; }

        public connection()
        {
            dbPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            dbPath = Path.Combine(dbPath, "carServiceDB.db3");
            db = new SQLiteConnection(dbPath);
        }
    }   
}