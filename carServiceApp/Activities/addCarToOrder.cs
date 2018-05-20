﻿using System;
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
    [Activity(Label = "addCarToOrder")]
    public class addCarToOrder : Activity
    {
        private Spinner spinner;
        private Button addNewCar;

        private string id;
        private List<string> carList = new List<string>();

        connection con = new connection();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.addCarToOrder);

            spinner = FindViewById<Spinner>(Resource.Id.spinnerUserCars);

            loadSpinner();
            addNewCar.Click += AddNewCar_Click;
        }

        private void AddNewCar_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(carDetails));
            StartActivity(intent);
        }

        private void loadSpinner()
        {
            carList.Clear();
            carList.Add("Odaberite stavku");
            FirebaseUser user = FirebaseAuth.GetInstance(loginActivity.app).CurrentUser;
            id = user.Uid;
            List<carDetailsSQL> getData = con.db.Query<carDetailsSQL>("SELECT * FROM carDetailsSQL WHERE uid = '"+id+"' ");
            foreach (var item in getData)
            {
                carList.Add(item.carName);
            }

            ArrayAdapter adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, carList);
            spinner.Adapter = adapter;
        }
    }
}