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
using carServiceApp.Activities;
using Firebase.Auth;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;

namespace carServiceApp.My_Classes.Database
{
    class addCar :DialogFragment
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

        public event EventHandler<onDialogClosed> onDialogClosedEvent;

        public View view;
        private bool allGood = true;
        createAppointment createAppointment = new createAppointment();
        connection con = new connection();

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.carDetails, container, false);

            markaVozila       = view.FindViewById<EditText>(Resource.Id.CDmarkaVozila);
            tipVozila         = view.FindViewById<EditText>(Resource.Id.CDTipVozila);
            godinaProizvodnje = view.FindViewById<EditText>(Resource.Id.CDgodiste);
            modelVozila       = view.FindViewById<EditText>(Resource.Id.CDmodelVozila);
            tipMotora         = view.FindViewById<EditText>(Resource.Id.CDvrstaGoriva);
            snagaMotora       = view.FindViewById<EditText>(Resource.Id.CDsnagaMotora);
            zapremninaMotora  = view.FindViewById<EditText>(Resource.Id.CDzapremninaMotora);
            carName           = view.FindViewById<EditText>(Resource.Id.CDimeVozila);
            saveCar           = view.FindViewById<Button>(Resource.Id.saveCar);

            FirebaseUser users = FirebaseAuth.GetInstance(loginActivity.app).CurrentUser;
            id = users.Uid;
            createAppointment.updateUser();

            saveCar.Click += SaveCar_Click;
            carName.FocusChange += CarName_FocusChange;
            return view;
        }

        private void CarName_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            carName.SetTextColor(Android.Graphics.Color.Black);
        }

        private void SaveCar_Click(object sender, EventArgs e)
        {
            if (checkIfAllInserted() == false) { return; }
            addCarInfo();
            if (allGood)
            {              
                this.Dismiss();
            }
        }

        private bool checkIfAllInserted()
        {
            if (markaVozila.Text == "" || tipVozila.Text == "" || godinaProizvodnje.Text == "" || modelVozila.Text == ""
                                       || tipMotora.Text == "" || snagaMotora.Text == "" || zapremninaMotora.Text == "" || carName.Text == "")
            {
                Toast.MakeText(view.Context, "Sva polja moraju biti popunjena!", ToastLength.Short).Show();
                return false;
            }
            else
            {
                return true;
            }
        }

        private void addCarInfo()
        {

            carDetailsSQL CarDetails = new carDetailsSQL();
            CarDetails CarDetailsFB = new CarDetails();

            CarDetails.markaVozila = markaVozila.Text;
            CarDetails.tipVozila = tipVozila.Text;
            CarDetails.godina = godinaProizvodnje.Text;
            CarDetails.modelVozila = modelVozila.Text;
            CarDetails.tipMotora = tipMotora.Text;
            CarDetails.snagaMotora = snagaMotora.Text;
            CarDetails.zapremninaMotora = zapremninaMotora.Text;
            CarDetails.carName = carName.Text;
            CarDetails.uid = id;

            CarDetailsFB.markaVozila = markaVozila.Text;
            CarDetailsFB.tipVozila = tipVozila.Text;
            CarDetailsFB.godina = godinaProizvodnje.Text;
            CarDetailsFB.modelVozila = modelVozila.Text;
            CarDetailsFB.tipMotora = tipMotora.Text;
            CarDetailsFB.snagaMotora = snagaMotora.Text;
            CarDetailsFB.zapremninaMotora = zapremninaMotora.Text;
            CarDetailsFB.uid = id;

            try
            {
                //insert user

                con.db.Insert(CarDetails);
                var firebase = new FirebaseClient(FirebaseURL);
                var item = firebase.Child("car").Child(id).Child(carName.Text).PutAsync(CarDetailsFB);
                Toast.MakeText(view.Context, "Uspješno ste dodali automobil", ToastLength.Long).Show();
            }
            catch (System.Exception)
            {
                    Toast.MakeText(view.Context, "Već ste dodali automobil s ovim skraćenim nazivom", ToastLength.Long).Show();
                    carName.SetTextColor(Android.Graphics.Color.Red);
            }

        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
        }

        public override void OnDismiss(IDialogInterface dialog)
        {
            onDialogClosedEvent.Invoke(this, new onDialogClosed(true));
            base.OnDismiss(dialog);
        }

    }


    public class onDialogClosed
    {
        public bool closed { get; set; }

        public onDialogClosed(bool IsClosed)
        {
            closed = IsClosed;
        }
    }
}