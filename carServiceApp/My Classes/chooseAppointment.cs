using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using carServiceApp.Activities;
using carServiceApp.My_Classes.Database;
using Firebase.Auth;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;
using Newtonsoft.Json;

namespace carServiceApp.My_Classes
{
    class chooseAppointment :DialogFragment
    {
        private View view;
        private string id;
        private string stringOrderID;
        private ListView ordersLV;
        private ImageButton refreshButton;
        private ProgressBar progressBar;
        private string tag;

        private List<orders> ordersList;
        private List<string> list = new List<string>();

        connection con = new connection();

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view          = inflater.Inflate(Resource.Layout.chooseAppointment, container, false);
            ordersLV      = view.FindViewById<ListView>(Resource.Id.appointmentList);
            ordersList    = new List<orders>();
            refreshButton = view.FindViewById<ImageButton>(Resource.Id.refreshAppointments);
            progressBar   = view.FindViewById<ProgressBar>(Resource.Id.CAprogressBar);

            tag = Tag;

            ordersList.Clear();
            getAppointments(tag);
        
          ordersLV.ItemClick += OrdersLV_ItemClick;
          refreshButton.Click += RefreshButton_Click;
            
            return view;
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            getAppointments(tag);
            progressBar.Visibility = ViewStates.Visible;
            refreshButton.Enabled = false;
        }

        private void OrdersLV_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Intent intent = new Intent(view.Context, typeof(myAppointments));
            intent.PutExtra("orderID", ordersList[e.Position].uid);
            StartActivity(intent);
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
        }


        public async void getAppointments(string tag)
        {
            var user = FirebaseAuth.Instance.CurrentUser;
            ordersList.Clear();

            string orderID;
            string id = user.Uid;

            var firebase = new FirebaseClient(loginActivity.FirebaseURL);
            connection con = new connection();

            int numOfOrders = 1;
            orderID = JsonConvert.SerializeObject(numOfOrders.ToString());

            List<orders> allOrders = new List<orders>();
            var getONumberOfOrders = await firebase.Child("order").Child(id).OnceAsync<orders>();
            foreach (var item in getONumberOfOrders)
            {
                allOrders.Add(new orders { carName = item.Object.carName });
                numOfOrders = allOrders.Count + 1;
                orderID = numOfOrders.ToString();
                orderID = JsonConvert.SerializeObject(orderID);

                if (tag == "fromMainActivity")
                {
                    var getOtherData = await firebase.Child("order").Child(id).Child(item.Key).OnceAsync<orders>();
                    foreach (var Otheritem in getOtherData)
                    {
                        stringOrderID = Otheritem.Object.id;
                        ordersList.Add(new orders { uid = Otheritem.Object.id, carName = Otheritem.Object.carName, datumKreiranja = Otheritem.Object.datumKreiranja.Substring(0, 10) });
                    }
                }

                if (tag != "fromMainActivity")
                {
                    var getOtherData = await firebase.Child("order").Child(id).Child(item.Key).OnceAsync<orders>();
                    foreach (var Otheritem in getOtherData)
                    {
                        stringOrderID = Otheritem.Object.id;
                        if (tag == Otheritem.Object.carName)
                        {
                            ordersList.Add(new orders { uid = Otheritem.Object.id, carName = Otheritem.Object.carName, datumKreiranja = Otheritem.Object.datumKreiranja.Substring(0, 10) });
                        }
                        
                    }
                }
            }

            listViewAdapter adapter = new listViewAdapter(view.Context, ordersList);
            ordersLV.Adapter = adapter;
            progressBar.Visibility = ViewStates.Invisible;
            refreshButton.Enabled = true;
        }

       
    }
}