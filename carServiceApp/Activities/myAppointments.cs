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

namespace carServiceApp.Activities
{
    [Activity(Label = "myAppointments")]
    public class myAppointments : Activity
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


            napomenaServisa.Text = "Serviser još uvijek nije dodao nikakvu napomenu";
        }
    }
}