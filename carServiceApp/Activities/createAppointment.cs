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

        private string user;
        private string id;
        private FirebaseAuth auth;
        private const string FirebaseURL = loginActivity.FirebaseURL;

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

            List<string> vrstaUslugeList = new List<string>();
            vrstaUslugeList.Add("Odaberite stavku");
            vrstaUslugeList.Add("Automehaničarska usluga");
            vrstaUslugeList.Add("Autolakirerska usluga");
            vrstaUslugeList.Add("Vulkanizerska usluga");
            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, vrstaUslugeList);
            vrstaUsluge.Adapter = adapter;

            await LoadData();
         
            next.Click += Next_Click;
           
        }
      

        private void Next_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(carDetails));
            StartActivity(intent);
        }

        private async Task LoadData()
        {
            FirebaseUser users = FirebaseAuth.GetInstance(loginActivity.app).CurrentUser;
            id = users.Uid;

            var firebase = new FirebaseClient(loginActivity.FirebaseURL);

            var items = await firebase.Child("users").Child(id).OnceAsync<Account>();

            foreach (var item in items)
            {
                Account user = new Account();
             
                user.uid = item.Object.uid;
                user.name = item.Object.name;
                user.lastName = item.Object.lastName;
                user.phone = item.Object.phone;
                user.email = item.Object.email;

                userInput_ime.Text = user.name;
                userInput_prezime.Text = user.lastName;
                userInput_broj.Text = user.phone;
                userInput_email.Text = user.email;
            }

        }

    }

  
}