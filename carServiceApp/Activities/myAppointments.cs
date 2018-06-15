using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

namespace carServiceApp.Activities
{
    [Activity(Label = "myAppointments")]
    public class myAppointments : Activity, IDialogInterfaceOnDismissListener
    {
        TextView napomenaServisa;
        TextView status;
        TextView automobil;
        TextView vrstaUsluge;
        TextView vrstaPosla;
        TextView brojNarudzbe;
        TextView datumServisa;
        TextView vrijemeServisa;
        TextView cijena;
        ProgressBar progressBar;

        private string orderID;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.myAppointments);

            napomenaServisa = FindViewById<TextView>(Resource.Id.napomenaServiseraTV);
            status          = FindViewById<TextView>(Resource.Id.statusTV);
            automobil       = FindViewById<TextView>(Resource.Id.carTV);
            vrstaUsluge     = FindViewById<TextView>(Resource.Id.VrstauslugeTV);
            vrstaPosla      = FindViewById<TextView>(Resource.Id.vrstaPoslaTV);
            brojNarudzbe    = FindViewById<TextView>(Resource.Id.brojNarudzbeTV);
            datumServisa    = FindViewById<TextView>(Resource.Id.datumServisaTV);
            vrijemeServisa  = FindViewById<TextView>(Resource.Id.vrijemeServisaTV);
            cijena          = FindViewById<TextView>(Resource.Id.cijenaTV);
            progressBar     = FindViewById<ProgressBar>(Resource.Id.progressBarMA);

            orderID = "\"" + Intent.GetStringExtra("orderID") + "\"";
            napomenaServisa.Text = "Serviser još uvijek nije dodao nikakvu napomenu";
            progressBar.Activated = true;

            if (IsOnline())
            {
                getOnlineDadata();
            }
            else
            {
                MainActivity.checkIfOnline(this, this, this);
            }
          
        }


        private async void getOnlineDadata()
        {
            var id = FirebaseAuth.GetInstance(loginActivity.app).CurrentUser.Uid;
            var firebase = new FirebaseClient(loginActivity.FirebaseURL);

            var data = await firebase.Child("order").Child(id).Child(orderID).OnceAsync<orders>();
            foreach (var item in data)
            {
                status.Text          = item.Object.status;
                cijena.Text          = item.Object.cijena;
                automobil.Text       = item.Object.carName;
                vrstaUsluge.Text     = item.Object.vrstaUsluge;
                vrstaPosla.Text      = item.Object.vrstaPosla;
                brojNarudzbe.Text    = item.Object.id;
                datumServisa.Text    = item.Object.datum.Substring(0, 10);
                vrijemeServisa.Text  = item.Object.vrijemeServisa;
                cijena.Text          = item.Object.cijena;
                if (item.Object.napomenaServisera != "") napomenaServisa.Text = item.Object.napomenaServisera;

                System.Threading.Thread.Sleep(2000); 
                progressBar.Activated = false;
                progressBar.Visibility = ViewStates.Invisible;
            }
        }

        public bool IsOnline()
        {
            var cm = (ConnectivityManager)GetSystemService(ConnectivityService);
            return cm.ActiveNetworkInfo == null ? false : cm.ActiveNetworkInfo.IsConnected;
        }

        public void OnDismiss(IDialogInterface dialog)
        {
            Intent intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
        }
    }
}