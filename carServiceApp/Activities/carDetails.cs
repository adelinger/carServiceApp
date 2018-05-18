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
using carServiceApp.My_Classes;
using carServiceApp.My_Classes.Database;
using Firebase.Auth;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;

namespace carServiceApp.Activities
{
    [Activity(Label = "carDetails")]
    public class carDetails : Activity
    {
        private string id;
        private string key;
        private FirebaseAuth auth;
        private const string FirebaseURL = loginActivity.FirebaseURL;

        private EditText markaVozila;
        private EditText tipVozila;
        private EditText godinaProizvodnje;
        private EditText modelVozila;
        private EditText tipMotora;
        private EditText snagaMotora;
        private EditText zapremninaMotora;
        private Button saveCar;
        private ProgressBar progressBar;

        createAppointment createAppointment = new createAppointment();
        connection con = new connection();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.carDetails);

            markaVozila       = FindViewById<EditText>(Resource.Id.CDmarkaVozila);
            tipVozila         = FindViewById<EditText>(Resource.Id.CDTipVozila);
            godinaProizvodnje = FindViewById<EditText>(Resource.Id.CDgodiste);
            modelVozila       = FindViewById<EditText>(Resource.Id.CDmodelVozila);
            tipMotora         = FindViewById<EditText>(Resource.Id.CDvrstaGoriva);
            snagaMotora       = FindViewById<EditText>(Resource.Id.CDsnagaMotora);
            zapremninaMotora  = FindViewById<EditText>(Resource.Id.CDzapremninaMotora);
            progressBar       = FindViewById<ProgressBar>(Resource.Id.progressBarCD);
            saveCar = FindViewById<Button>(Resource.Id.saveCar);

            createAppointment.updateUser();
        }

        private async void addCarInfo()
        {
            FirebaseUser users = FirebaseAuth.GetInstance(loginActivity.app).CurrentUser;
            id = users.Uid;

            carDetailsSQL CarDetails = new carDetailsSQL();
            My_Classes.Database.carDetails CarDetailsFB = new My_Classes.Database.carDetails();

            CarDetails.markaVozila      = markaVozila.Text;
            CarDetails.tipVozila        = tipVozila.Text;
            CarDetails.godina           = godinaProizvodnje.Text;
            CarDetails.modelVozila      = modelVozila.Text;
            CarDetails.tipMotora        = tipMotora.Text;
            CarDetails.snagaMotora      = snagaMotora.Text;
            CarDetails.zapremninaMotora = zapremninaMotora.Text;
            CarDetails.uid = id;

            CarDetailsFB.markaVozila = markaVozila.Text;
            CarDetailsFB.tipVozila = tipVozila.Text;
            CarDetailsFB.godina = godinaProizvodnje.Text;
            CarDetailsFB.modelVozila = modelVozila.Text;
            CarDetailsFB.tipMotora = tipMotora.Text;
            CarDetailsFB.snagaMotora = snagaMotora.Text;
            CarDetailsFB.zapremninaMotora = zapremninaMotora.Text;

           // con.db.CreateTable<CarDetailsSQL>(); //creating table in offline database
           // con.db.Insert(CarDetails);  //inserting into offline database

           // var firebase = new FirebaseClient(FirebaseURL);
           // var item = firebase.Child("car").Child(id).PostAsync<CarDetails>(CarDetailsFB);  //inserting into online database

        }

    }
}