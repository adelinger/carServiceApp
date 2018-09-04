using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using carServiceApp.My_Classes;
using carServiceApp.My_Classes.Database;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;
using Java.Util;
using Newtonsoft.Json;
using static Android.Widget.AdapterView;

namespace carServiceApp.Activities
{
    [Activity(Label = "Dodaj svoje podatke")]
    public class createAppointment : Activity, IDialogInterfaceOnDismissListener
    {
        private Spinner  vrstaUsluge;
        private Spinner  vrstaPosla;
        public  EditText userInput_ime;
        private EditText userInput_prezime;
        private EditText userInput_broj;
        private EditText userInput_email; 
        private EditText userInpu_ulicaibr;
        private EditText userInput_mjesto;
        private Button   next;

        List<string> vrstaUslugeList = new List<string>();
        List<string> vrstaPoslaList = new List<string> ();

        private string user;
        private bool updateRequested;
        private string id;
        private string key;
        private FirebaseAuth auth;
        private const string FirebaseURL = loginActivity.FirebaseURL;

        connection con = new connection();

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
 
            SetContentView(Resource.Layout.createAppointment);

            vrstaUsluge       = FindViewById<Spinner >(Resource.Id.vrstaUsluge);
            vrstaPosla        = FindViewById<Spinner >(Resource.Id.vrstaPosla);
            userInput_ime     = FindViewById<EditText>(Resource.Id.CAinputIme);
            userInput_prezime = FindViewById<EditText>(Resource.Id.CAinputPrezime);
            userInput_broj    = FindViewById<EditText>(Resource.Id.CAinputBrojTelefona);
            userInput_email   = FindViewById<EditText>(Resource.Id.CAinputEmail);
            userInpu_ulicaibr = FindViewById<EditText>(Resource.Id.CAinputUlicaIBroj);
            userInput_mjesto  = FindViewById<EditText>(Resource.Id.CAinputMjesto);
            next              = FindViewById<Button  >(Resource.Id.CANextButton);  

            auth = FirebaseAuth.GetInstance(loginActivity.app);
            user = auth.CurrentUser.Uid;       

            vrstaUslugeList.Add("Odaberite stavku");
            vrstaPoslaList.Add ("Odaberite stavku");

            LoadData();

            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, vrstaUslugeList);
            vrstaUsluge.Adapter = adapter;

            next.Click += Next_Click;
            vrstaUsluge.ItemSelected += VrstaUsluge_ItemSelected;
           
        }
      
        public bool IsOnline()
        {
            var cm = (ConnectivityManager)GetSystemService(ConnectivityService);
            return cm.ActiveNetworkInfo == null ? false : cm.ActiveNetworkInfo.IsConnected;
        }

        private void VrstaUsluge_ItemSelected(object sender, ItemSelectedEventArgs e)
        {
            string selected = vrstaUsluge.GetItemAtPosition(e.Position).ToString();
          
            if (selected != "Odaberite stavku")
            {
                vrstaPoslaList.Clear();
                vrstaPoslaList.Add("Odaberite stavku");
                List<uslugeSQL> getData = con.db.Query<uslugeSQL>("SELECT * FROM uslugeSQL WHERE type = '" + selected + "' ");
                foreach (var item in getData)
                {
                    vrstaPoslaList.Add(item.name);
                }

                ArrayAdapter<string> adapter2 = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, vrstaPoslaList);
                vrstaPosla.Adapter = adapter2;
            }
            if (selected == "Odaberite stavku")
            {
                vrstaPoslaList.RemoveRange(0, vrstaPoslaList.Count);
                vrstaPoslaList.Add("Odaberite stavku");

                ArrayAdapter<string> adapter2 = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, vrstaPoslaList);
                vrstaPosla.Adapter = adapter2;
            }
            
        }

        protected override void OnResume()
        {
            updateRequested = Intent.GetBooleanExtra("updateRequested", false);
            base.OnResume();
        }
        
        private async void Next_Click(object sender, EventArgs e)
        {
            if (userInput_ime.Text == "" || userInput_prezime.Text == "" || userInput_broj.Text == "" || userInput_email.Text == "" || userInput_mjesto.Text == "" || userInpu_ulicaibr.Text == "")
            {
                Toast.MakeText(this, "Sva polja moraju biti ispunjena!", ToastLength.Short).Show();
                return;
            }
            if (vrstaUsluge.SelectedItem.ToString() == "Odaberite stavku")
            {
                Toast.MakeText(this, "Vrsta usluge mora biti odabrana", ToastLength.Short).Show();
                return;
            }
            if (vrstaPosla.SelectedItem.ToString() == "Odaberite stavku")
            {
                Toast.MakeText(this, "Vrsta posla mora biti odabrana", ToastLength.Short).Show();
                return;
            }
           
            if (!IsOnline())
            {
                MainActivity.checkIfOnline(this, this, this);
                return;
            }

            FirebaseUser users = FirebaseAuth.GetInstance(loginActivity.app).CurrentUser;
            id = users.Uid;

            var firebase = new FirebaseClient(loginActivity.FirebaseURL);

            Account user = new Account();
            user.name     = userInput_ime.Text;
            user.lastName = userInput_prezime.Text;
            user.email    = userInput_email.Text;
            user.phone    = userInput_broj.Text;
            user.city     = userInput_mjesto.Text;
            user.adress   = userInpu_ulicaibr.Text;
            user.uid      = id;
            var data      = await firebase.Child("users").Child(id).OnceAsync<Account>();
            foreach (var item in data)
            {
                key = item.Key;
            }

            var items = firebase.Child("users").Child(id).Child(key).PutAsync<Account>(user);

            con.db.Execute("UPDATE User SET name = '" + user.name + "', lastName = '" + user.lastName + "', phone = '" + user.phone + "', " +
               "email = '" + user.email + "', city = '" + user.city + "', adress = '" + user.adress + "' WHERE uid = '" + id + "' ");

            if (updateRequested)
            {
                Intent intent = new Intent(this, typeof(confirmOrder)).SetFlags(ActivityFlags.ReorderToFront);
                intent.PutExtra("vrstaUsluge", vrstaUsluge.SelectedItem.ToString());
                intent.PutExtra("vrstaPosla", vrstaPosla.SelectedItem.ToString());
                StartActivity(intent);
            }
            else
            {
                Intent intent = new Intent(this, typeof(addCarToOrder));
                intent.PutExtra("vrstaUsluge", vrstaUsluge.SelectedItem.ToString());
                intent.PutExtra("vrstaPosla", vrstaPosla.SelectedItem.ToString());
                StartActivity(intent);
            }
        }

        private async void LoadData()
        {
            FirebaseUser users = FirebaseAuth.GetInstance(loginActivity.app).CurrentUser;
            id = users.Uid;

            List<services> getServices = con.db.Query<services>("SELECT * FROM services");

            foreach (var item in getServices)
            {
                vrstaUslugeList.Add(item.name);
            }

            List<User> data = con.db.Query<User>("SELECT * FROM User WHERE uid = '" + id + "'");
            foreach (var item in data)
            {
                userInput_ime.Text     = item.name;
                userInput_prezime.Text = item.lastName;
                userInput_broj.Text    = item.phone;
                userInput_email.Text   = item.email;
                userInput_mjesto.Text  = item.city;
                userInpu_ulicaibr.Text = item.adress;
            }
          
        }

        public async void updateUser()
        {        
            User user = new User();
            CarDetails car = new CarDetails();
            var firebase = new FirebaseClient(loginActivity.FirebaseURL);
            FirebaseUser users = FirebaseAuth.GetInstance(loginActivity.app).CurrentUser;
            id = users.Uid;

            var data = await firebase.Child("users").Child(id).OnceAsync<Account>();
            foreach (var item in data)
            {
                user.name     = item.Object.name;
                user.lastName = item.Object.lastName;
                user.phone    = item.Object.phone;
                user.email    = item.Object.email;
                user.city     = item.Object.city;
                user.adress   = item.Object.adress;
            }

            con.db.Execute("UPDATE User SET name = '" + user.name + "', lastName = '" + user.lastName + "', phone = '" + user.phone + "', " +
                "email = '" + user.email + "', city = '" + user.city + "', adress = '" + user.adress + "' WHERE uid = '" + id + "' ");
       
        }

        public void OnDismiss(IDialogInterface dialog)
        {
            dialog.Dispose();           
            Intent intent = new Intent(this, typeof(loginActivity));
            StartActivity(intent);
        }
    }

  
}