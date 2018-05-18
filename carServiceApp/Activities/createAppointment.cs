using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
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

namespace carServiceApp.Activities
{
    [Activity(Label = "createAppointment")]
    public class createAppointment : Activity
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
            await LoadData();
            
            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, vrstaUslugeList);
            vrstaUsluge.Adapter = adapter;
            ArrayAdapter<string> adapter2 = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, vrstaPoslaList);
            vrstaPosla.Adapter = adapter2;

            next.Click += Next_Click;
           
        }
      

        private async void Next_Click(object sender, EventArgs e)
        {
            if (userInput_ime.Text == "" || userInput_prezime.Text == "" || userInput_broj.Text == "" || userInput_email.Text == "" || userInput_mjesto.Text == "" || userInpu_ulicaibr.Text == "")
            {
                Toast.MakeText(this, "Sva polja moraju biti ispunjena!", ToastLength.Short).Show();
                return;
            }
            if (vrstaPosla.SelectedItem.ToString() == "Odaberite stavku")
            {
                Toast.MakeText(this, "Vrsta posla mora biti odabrana", ToastLength.Short).Show();
                return;
            }
            if (vrstaUsluge.SelectedItem.ToString() == "Odaberite stavku")
            {
                Toast.MakeText(this, "Vrsta usluge mora biti odabrana", ToastLength.Short).Show();
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

            Intent intent = new Intent(this, typeof(carDetails));
            StartActivity(intent);
        }

        private async Task LoadData()
        {
            FirebaseUser users = FirebaseAuth.GetInstance(loginActivity.app).CurrentUser;
            id = users.Uid;

            var firebase = new FirebaseClient(loginActivity.FirebaseURL);


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

            List<uslugeSQL> getData = con.db.Query<uslugeSQL>("SELECT * FROM uslugeSQL");
            foreach (var item in getData)
            {
                vrstaPoslaList.Add(item.name);
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
       

    }

  
}