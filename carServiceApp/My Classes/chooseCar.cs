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

namespace carServiceApp.My_Classes
{
    class chooseCar :DialogFragment
    {
        private View view;
        private ListView carList;
        private Button addNewCar;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.chooseCar, container, false);
            carList= view.FindViewById<ListView>(Resource.Id.carList);
            addNewCar = view.FindViewById<Button>(Resource.Id.addNewCar);
            

            List<string> emptyList = new List<string>();
            emptyList.Add("Još niste dodali niti jedno vozilo.");            
            if (carList.Count == 0)
            {
                ArrayAdapter adapter = new ArrayAdapter(view.Context, Android.Resource.Layout.SimpleListItem1, emptyList);
                carList.Adapter = adapter;
            }

            addNewCar.Click += AddNewCar_Click;

            return view;
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


    }
}