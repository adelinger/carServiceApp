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
using Java.Lang;

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
        private EditText carName;
        private Button saveCar;

        createAppointment createAppointment = new createAppointment();
        connection con = new connection();
        protected override async void OnCreate(Bundle savedInstanceState)
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
            carName           = FindViewById<EditText>(Resource.Id.CDimeVozila);
            saveCar           = FindViewById<Button>(Resource.Id.saveCar);
            
            createAppointment.updateUser();
            saveCar.Click += SaveCar_Click;

        }

        private async void SaveCar_Click(object sender, EventArgs e)
        {
            await addCarInfo();
            OnBackPressed();

            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            chooseCar chooseCar = new chooseCar();
            chooseCar.Show(transaction, "dialog fragment");
        }

        private async Task addCarInfo()
        {
            FirebaseUser users = FirebaseAuth.GetInstance(loginActivity.app).CurrentUser;
            id = users.Uid;

            carDetailsSQL CarDetails = new carDetailsSQL();
            CarDetails  CarDetailsFB = new CarDetails();

            CarDetails.markaVozila        = markaVozila.Text;
            CarDetails.tipVozila          = tipVozila.Text;
            CarDetails.godina             = godinaProizvodnje.Text;
            CarDetails.modelVozila        = modelVozila.Text;
            CarDetails.tipMotora          = tipMotora.Text;
            CarDetails.snagaMotora        = snagaMotora.Text;
            CarDetails.zapremninaMotora   = zapremninaMotora.Text;
            CarDetails.carName            = carName.Text;
            CarDetails.uid = id;

            CarDetailsFB.markaVozila      = markaVozila.Text;
            CarDetailsFB.tipVozila        = tipVozila.Text;
            CarDetailsFB.godina           = godinaProizvodnje.Text;
            CarDetailsFB.modelVozila      = modelVozila.Text;
            CarDetailsFB.tipMotora        = tipMotora.Text;
            CarDetailsFB.snagaMotora      = snagaMotora.Text;
            CarDetailsFB.zapremninaMotora = zapremninaMotora.Text;
            CarDetailsFB.uid              = id;

            try
            {
                con.db.Insert(CarDetails);
                var firebase = new FirebaseClient(FirebaseURL);
                var item = firebase.Child("car").Child(carName.Text).PostAsync(CarDetailsFB);
            }
            catch (System.Exception)
            {
                Toast.MakeText(this, "Neuspješno", ToastLength.Long).Show();
            }

            Toast.MakeText(this, "Uspješno ste dodali automobil", ToastLength.Long).Show();
        }

       
    }
}