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
        private string getCarName;
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

        private bool allGood = true;

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
            carName           = FindViewById<EditText>(Resource.Id.CDimeVozila);
            saveCar           = FindViewById<Button>(Resource.Id.saveCar);

            FirebaseUser users = FirebaseAuth.GetInstance(loginActivity.app).CurrentUser;
            id = users.Uid;

            getCarName = Intent.GetStringExtra("carName");
            if(getCarName != "")
            {
                getCarData(getCarName);
            }
            
            createAppointment.updateUser();
            saveCar.Click += SaveCar_Click;
            carName.FocusChange += CarName_FocusChange;
        }

        private void CarName_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            carName.SetTextColor(Android.Graphics.Color.Black);
        }

        private void SaveCar_Click(object sender, EventArgs e)
        {
            if(checkIfAllInserted() == false) { return; }
            addCarInfo();
            if (allGood) { OnBackPressed(); }
        }

        private bool checkIfAllInserted()
        {
            if (markaVozila.Text == "" || tipVozila.Text == "" || godinaProizvodnje.Text =="" || modelVozila.Text == "" 
                                       || tipMotora.Text =="" || snagaMotora.Text == "" || zapremninaMotora.Text == "" ||carName.Text == "")
            {
                Toast.MakeText(this, "Sva polja moraju biti popunjena!", ToastLength.Short).Show();
                return false;
            }
            else
            {
                return true;
            }
        }

        private void getCarData (string name)
        {
            List<carDetailsSQL> getData = con.db.Query<carDetailsSQL>("SELECT * FROM carDetailsSQL WHERE carName = '" + name + "' ");
            foreach (var item in getData)
            {
                markaVozila.Text       = item.markaVozila;
                tipVozila.Text         = item.tipVozila;
                godinaProizvodnje.Text = item.godina;
                modelVozila.Text       = item.modelVozila;
                tipMotora.Text         = item.tipMotora;
                snagaMotora.Text       = item.snagaMotora;
                zapremninaMotora.Text  = item.zapremninaMotora;
                carName.Text           = item.carName;
                carName.Enabled        = false;
            }
        }

        private void addCarInfo()
        {

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
                //insert user

                con.db.Insert(CarDetails);
                var firebase = new FirebaseClient(FirebaseURL);
                var item = firebase.Child("car").Child(id).Child(carName.Text).PutAsync(CarDetailsFB);
                Toast.MakeText(this, "Uspješno ste dodali automobil", ToastLength.Long).Show();
            }
            catch (System.Exception)
            {
                if (!string.IsNullOrEmpty(getCarName))
                {
                    var firebase = new FirebaseClient(FirebaseURL);
                    var item = firebase.Child("car").Child(id).Child(carName.Text).PutAsync<CarDetails>(CarDetailsFB);

                    con.db.Execute("UPDATE carDetailsSQL SET markaVozila = '" + CarDetails.markaVozila + "', tipVozila = '" + CarDetails.tipVozila + "', godina = '" + CarDetails.godina + "'," +
                        " modelVozila = '" + CarDetails.modelVozila + "', tipMotora = '" + CarDetails.tipMotora + "', snagaMotora = '" + CarDetails.snagaMotora + "'," +
                        " zapremninaMotora = '" + CarDetails.zapremninaMotora + "' WHERE uid = '" + id + "' ");
                    Toast.MakeText(this, "Spremljeno", ToastLength.Long).Show();
                }
                    
                    if(string.IsNullOrEmpty(getCarName))
                {
                    Toast.MakeText(this, "Već ste dodali automobil s ovim skraćenim nazivom", ToastLength.Long).Show();
                    carName.SetTextColor(Android.Graphics.Color.Red);
                    allGood = false;
                }       

            }
           
        }

       
    }
}