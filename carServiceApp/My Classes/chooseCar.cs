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
using carServiceApp.Activities;
using carServiceApp.My_Classes.Database;
using Firebase.Auth;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;

namespace carServiceApp.My_Classes
{
    class chooseCar :DialogFragment
    {
        private View view;
        private ListView carList;
        private Button addNewCar;
        private List<string> carsFromDB = new List<string>();

        const string firebaseURL = loginActivity.FirebaseURL;

        private string id;

        connection con = new connection();

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.chooseCar, container, false);
            carList= view.FindViewById<ListView>(Resource.Id.carList);
            addNewCar = view.FindViewById<Button>(Resource.Id.addNewCar);

            getCars();

            List<string> emptyList = new List<string>();
            emptyList.Add("Još niste dodali niti jedno vozilo.");            
            if (carList.Count == 0)
            {
                ArrayAdapter adapter = new ArrayAdapter(view.Context, Android.Resource.Layout.SimpleListItem1, emptyList);
                carList.Adapter = adapter;
            }

            addNewCar.Click += AddNewCar_Click;
            carList.ItemLongClick += CarList_ItemLongClick;

            return view;
        }

        private void CarList_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            AlertDialog.Builder dialog = new AlertDialog.Builder(view.Context);
            dialog.SetTitle("Potvrda brisanja");
            dialog.SetMessage("Jeste li sigurni da želite obrisati označeni automobil?");
            dialog.SetPositiveButton("Obriši", (senderAlert, args) =>
            {
                string selected = carList.GetItemAtPosition(e.Position).ToString();
                var firebase = new FirebaseClient(firebaseURL);
                
               
            });
            dialog.SetNegativeButton("Odustani", (senderAlert, args) =>
            {
                dialog.Dispose();
            });
        }

        private void AddNewCar_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(view.Context,typeof(carDetails));
            StartActivity(intent);
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
        }

        public void getCars ()
        {
            FirebaseUser users = FirebaseAuth.GetInstance(loginActivity.app).CurrentUser;
            id = users.Uid;

            List<carDetailsSQL> getCarsData = con.db.Query<carDetailsSQL>("SELECT * FROM carDetailsSQL");
            foreach (var item in getCarsData)   
            {
                carsFromDB.Add(item.markaVozila + ", " + item.modelVozila + ", " + item.godina);
            }
            ArrayAdapter adapter = new ArrayAdapter(view.Context, Android.Resource.Layout.SimpleListItem1, carsFromDB);
            carList.Adapter = adapter;
        }


    }
}