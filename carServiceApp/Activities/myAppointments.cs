using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using carServiceApp.My_Classes;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using carServiceApp.My_Classes.Database;
using Firebase.Auth;
using Firebase.Xamarin.Database;
using Newtonsoft.Json;
using Firebase.Xamarin.Database.Query;
using System.Threading;
using Firebase.Database;

namespace carServiceApp.Activities
{
    [Activity(Label = "Moji sastanci")]
    public class myAppointments : Activity, IDialogInterfaceOnDismissListener
    {
        TextView napomenaServisa;
        TextView status;
        TextView automobil;
        TextView vrstaUsluge;
        TextView vrstaPosla;
        TextView brojNarudzbe;
        EditText datumServisa;
        EditText vrijemeServisa;
        EditText cijena;
        ProgressBar progressBar;
        Button applyOrChange;
        ImageButton refreshButton;

        orders order = new orders();
        string objectID;

        private string orderID;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.myAppointments);
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            napomenaServisa = FindViewById<TextView>(Resource.Id.napomenaServiseraTV);
            status          = FindViewById<TextView>(Resource.Id.statusTV);
            automobil       = FindViewById<TextView>(Resource.Id.carTV);
            vrstaUsluge     = FindViewById<TextView>(Resource.Id.VrstauslugeTV);
            vrstaPosla      = FindViewById<TextView>(Resource.Id.vrstaPoslaTV);
            brojNarudzbe    = FindViewById<TextView>(Resource.Id.brojNarudzbeTV);
            datumServisa    = FindViewById<EditText>(Resource.Id.datumServisaTV);
            vrijemeServisa  = FindViewById<EditText>(Resource.Id.vrijemeServisaTV);
            cijena          = FindViewById<EditText>(Resource.Id.cijenaTV);
            progressBar     = FindViewById<ProgressBar>(Resource.Id.progressBarMA);
            applyOrChange   = FindViewById<Button>(Resource.Id.changeOrApplyButton);
            refreshButton   = FindViewById<ImageButton>(Resource.Id.refreshMyAppointments);

            orderID = "\"" + Intent.GetStringExtra("orderID") + "\"";
            progressBar.Activated = true;
            datumServisa.Enabled = false;
            vrijemeServisa.Enabled = false;
            applyOrChange.Enabled = false;
        
            if (IsOnline())
            {
                getOnlineDadata();
            }
            else
            {
                MainActivity.checkIfOnline(this, this, this);
            }

            applyOrChange.Click += ApplyOrChange_Click;
            refreshButton.Click += RefreshButton_Click;
            datumServisa.Click += DatumServisa_Click;
            vrijemeServisa.Click += VrijemeServisa_Click;
        }


        private void VrijemeServisa_Click(object sender, EventArgs e)
        {
            timePicker timePicker = new timePicker();
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            timePicker.Show(transaction, "tag");
            timePicker.OnTimePickedEvent += TimePicker_OnTimePickedEvent;
        }

        private void DatumServisa_Click(object sender, EventArgs e)
        {
            calendarView calendar = new calendarView();
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            calendar.Show(transaction, "tag");
            calendar.onDatePickedEvent += Calendar_onDatePickedEvent;          
        }


        private void TimePicker_OnTimePickedEvent(object sender, OnTimeSelectedArgs e)
        {
            vrijemeServisa.Text = e.hourSelected + ":" + " " + e.minutesSelected;

            AlertDialog.Builder dialogBuilder = new AlertDialog.Builder(this);
            dialogBuilder.SetMessage("Odabrani datum i vrijeme su: " + datumServisa.Text + ", " + vrijemeServisa.Text);
            dialogBuilder.SetPositiveButton("Potvrdi", (senderAlert, args) => {
                var id = FirebaseAuth.GetInstance(loginActivity.app).CurrentUser.Uid;
                var firebase = new FirebaseClient(loginActivity.FirebaseURL);

                order.status         = "Prijedlog korisnika";
                order.datumServisa   = datumServisa.Text;
                order.vrijemeServisa = vrijemeServisa.Text;

                try
                {
                    var data = firebase.Child("order").Child(id).Child(orderID).Child(objectID).PutAsync<orders>(order);
                    status.Text = "Prijedlog korisnika";
                }
                catch (Exception)
                {

                    throw;
                }
                refreshButton.PerformClick();
                getOnlineDadata();
                datumServisa.Enabled   = false;
                vrijemeServisa.Enabled = false;
                dialogBuilder.Dispose();
               
            });
            dialogBuilder.SetNegativeButton("Odustani", (senderAlert, args) => {
                dialogBuilder.Dispose();
                getOnlineDadata();               
            });

            AlertDialog alertDialog = dialogBuilder.Create();
            alertDialog.Show();
        }


        private void Calendar_onDatePickedEvent(object sender, onDatePickedEventArgs e)
        {
            datumServisa.Text = e.datePicked;
            vrijemeServisa.PerformClick();
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            progressBar.Enabled = true;
            progressBar.Visibility = ViewStates.Visible;
            getOnlineDadata();
        }

        private void ApplyOrChange_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder dialogBuilder = new AlertDialog.Builder(this).SetOnDismissListener(this);
            dialogBuilder.SetMessage("Želite li Potvrditi ili predložiti promjene");
            dialogBuilder.SetPositiveButton("Potvrdi", (senderAlert, args) => {

                var id = FirebaseAuth.GetInstance(loginActivity.app).CurrentUser.Uid;
                var firebase = new FirebaseClient(loginActivity.FirebaseURL);

                status.Text  = "Dogovoreno";
                order.status = "Dogovoreno";
                order.datumServisa = datumServisa.Text;
                order.vrijemeServisa = vrijemeServisa.Text;

                status.SetTextColor(Android.Graphics.Color.DarkOliveGreen);
                applyOrChange.Enabled = false;
              
                var data = firebase.Child("order").Child(id).Child(orderID).Child(objectID).PutAsync<orders>(order);
                getOnlineDadata();              
                dialogBuilder.Dispose();
            });
            dialogBuilder.SetNegativeButton("Odustani", (senderAlert, args) => {
                dialogBuilder.Dispose();
            });
            dialogBuilder.SetNeutralButton("Predloži izmjenu", (senderAlert, args) => {
                datumServisa.Text = "";
                datumServisa.Enabled = true;
                datumServisa.PerformClick();

                vrijemeServisa.Text = "";
                vrijemeServisa.Enabled = true;
                dialogBuilder.Dispose();
            });

            AlertDialog alertDialog = dialogBuilder.Create();
            alertDialog.Show();
        }

        private async void getOnlineDadata()
        {
            refreshButton.Enabled = false;
            refreshButton.Activated = false;
            refreshButton.Visibility = ViewStates.Gone;

            var id = FirebaseAuth.GetInstance(loginActivity.app).CurrentUser.Uid;
            var firebase = new FirebaseClient(loginActivity.FirebaseURL);

            var data = await firebase.Child("order").Child(id).Child(orderID).OnceAsync<orders>();
        
            foreach (var item in data)
            {
                objectID = item.Key;

                order.cijena            = item.Object.cijena;
                order.carName           = item.Object.carName;
                order.vrstaUsluge       = item.Object.vrstaUsluge;
                order.vrstaPosla        = item.Object.vrstaPosla;
                order.id                = item.Object.id;
                order.cijena            = item.Object.cijena;
                order.datumKreiranja    = item.Object.datumKreiranja;
                order.dijelovi          = item.Object.dijelovi;
                order.opisKvara         = item.Object.opisKvara;
                order.vucnaSluzba       = item.Object.vucnaSluzba;
                order.uid               = item.Object.uid;
                order.napomenaServisera = item.Object.napomenaServisera;
                order.pozeljniDatum     = item.Object.pozeljniDatum;

                status.Text          = item.Object.status;
                cijena.Text          = item.Object.cijena;
                automobil.Text       = item.Object.carName;
                vrstaUsluge.Text     = item.Object.vrstaUsluge;
                vrstaPosla.Text      = item.Object.vrstaPosla;
                brojNarudzbe.Text    = item.Object.id;
                datumServisa.Text    = item.Object.datumServisa;
                vrijemeServisa.Text  = item.Object.vrijemeServisa;
                cijena.Text          = item.Object.cijena;
                if (item.Object.napomenaServisera != "") napomenaServisa.Text = item.Object.napomenaServisera;

                progressBar.Activated = false;
                progressBar.Visibility = ViewStates.Invisible;
                refreshButton.Enabled = true;
                refreshButton.Visibility = ViewStates.Visible;

                if (status.Text != "Dogovoreno") { applyOrChange.Enabled = true; applyOrChange.SetBackgroundColor(Android.Graphics.Color.Rgb(192, 57, 43)); }
                if (status.Text == "Dogovoreno"){ status.SetTextColor(Android.Graphics.Color.DarkOliveGreen); applyOrChange.Enabled = false; applyOrChange.SetBackgroundColor(Android.Graphics.Color.LightSlateGray);}          
                if (status.Text == "Prijedlog korisnika")  { applyOrChange.SetBackgroundColor(Android.Graphics.Color.LightSlateGray); applyOrChange.Enabled = false; }
                if (status.Text == "Prijedlog servisa") { applyOrChange.Enabled = true; applyOrChange.SetBackgroundColor(Android.Graphics.Color.Rgb(192, 57, 43)); }
                if (status.Text == "Završeno") { applyOrChange.Enabled = false; applyOrChange.SetBackgroundColor(Android.Graphics.Color.LightSlateGray);}
                if (status.Text == "Kreirano") { applyOrChange.Enabled = false; applyOrChange.SetBackgroundColor(Android.Graphics.Color.LightSlateGray); }
            }
        }

        public bool IsOnline()
        {
            var cm = (ConnectivityManager)GetSystemService(ConnectivityService);
            return cm.ActiveNetworkInfo == null ? false : cm.ActiveNetworkInfo.IsConnected;
        }

        public void OnDismiss(IDialogInterface dialog)
        {
            refreshButton.PerformClick();
        }

      
    }
}