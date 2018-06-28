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
        private string connectionStatus;

        private List<orders> ordersList;
        private List<string> list = new List<string>();

        connection con = new connection();

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view          = inflater.Inflate(Resource.Layout.chooseAppointment, container, false);
            ordersLV      = view.FindViewById<ListView>(Resource.Id.appointmentList);
            ordersList    = new List<orders>();
            refreshButton = view.FindViewById<ImageButton>(Resource.Id.refreshAppointments);

            connectionStatus = Tag;

            updateIfOnline();

            ordersList.Clear();
            getAppointments();

            ordersLV.ItemClick += OrdersLV_ItemClick;
            refreshButton.Click += RefreshButton_Click;
            return view;
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            updateIfOnline();
            ordersList.Clear();
            getAppointments();
        }

        private void OrdersLV_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Intent intent = new Intent(view.Context, typeof(myAppointments));
            intent.PutExtra("orderID", stringOrderID);
            StartActivity(intent);
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
        }

        public void updateIfOnline ()
        {
            if (connectionStatus == "Online")
            {
                updateAppointments();
            }
        }

        public void getAppointments()
        {
            var user = FirebaseAuth.Instance.CurrentUser;
            id = user.Uid;
            ordersList.Clear();
          
            List<orders> data = con.db.Query<orders>("SELECT * FROM orders WHERE uid = '" + id + "' ");
            
            foreach (var item in data)
            {
                stringOrderID = item.id;
                ordersList.Add(new orders { uid = item.id, carName = item.carName, datum = item.datum.Substring(0, 10) });
            }
            listViewAdapter adapter = new listViewAdapter(view.Context, ordersList);
            ordersLV.Adapter = adapter;
        }

        public static async void updateAppointments ()
        {
            string orderID;
            var user = FirebaseAuth.Instance.CurrentUser;
            string id = user.Uid;

            var firebase = new FirebaseClient(loginActivity.FirebaseURL);
            connection con = new connection();
            orders order = new orders();

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

                var getOtherData = await firebase.Child("order").Child(id).Child(item.Key).OnceAsync<orders>();
                foreach (var Otheritem in getOtherData)
                {
                    con.db.Execute("INSERT OR IGNORE INTO orders (carName, datum, dijelovi, id, opisKvara, pozeljniDatum, uid, vrstaPosla, vrstaUsluge, vucnaSluzba) VALUES ('" + Otheritem.Object.carName + "'," +
                    " '" + Otheritem.Object.datum + "', '" + Otheritem.Object.dijelovi + "', '" + Otheritem.Object.id + "', '" + Otheritem.Object.opisKvara + "','" + Otheritem.Object.pozeljniDatum + "' , '" + Otheritem.Object.uid + "', " +
                    "'" + Otheritem.Object.vrstaPosla + "' ,'" + Otheritem.Object.vrstaUsluge + "' ,'" + Otheritem.Object.vucnaSluzba + "') ");
                }
            }

        }
    }
}