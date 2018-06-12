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
        private ListView ordersLV;

        private List<orders> ordersList;
        private List<string> list = new List<string>();

        connection con = new connection();

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.chooseAppointment, container, false);
            ordersLV = view.FindViewById<ListView>(Resource.Id.appointmentList);
            ordersList = new List<orders>();

            updateAppointments();
            ordersList.Clear();
            getAppointments();
            ordersLV.ItemClick += OrdersLV_ItemClick;

            return view;
        }

        private void OrdersLV_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Intent intent = new Intent(view.Context, typeof(myAppointments));
            StartActivity(intent);
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
        }

        public void getAppointments()
        {
            var user = FirebaseAuth.Instance.CurrentUser;
            id = user.Uid;
            ordersList.Clear();
          
            List<orders> data = con.db.Query<orders>("SELECT * FROM orders WHERE uid = '" + id + "' ");
            
            foreach (var item in data)
            {
                ordersList.Add(new orders { uid = item.id, carName = item.carName, datum = item.datum.Substring(0, 10) });
            }
            listViewAdapter adapter = new listViewAdapter(view.Context, ordersList);
            ordersLV.Adapter = adapter;
        }

        public static async void updateAppointments ()
        {       
            var user = FirebaseAuth.Instance.CurrentUser;
            string id = user.Uid;

            var firebase = new FirebaseClient(loginActivity.FirebaseURL);
            connection con = new connection();

            var data = await firebase.Child("order").Child(id).OnceAsync<orders>();
            foreach (var item in data)
            {
                con.db.Execute("INSERT OR IGNORE INTO orders (carName, datum, dijelovi, id, opisKvara, pozeljniDatum, uid, vrstaPosla, vrstaUsluge, vucnaSluzba) VALUES ('" + item.Object.carName + "'," +
                    " '" + item.Object.datum + "', '" + item.Object.dijelovi + "', '" + item.Object.id + "', '" + item.Object.opisKvara + "','"+item.Object.pozeljniDatum+"' , '" + item.Object.uid + "', " +
                    "'" + item.Object.vrstaPosla +"' ,'"+item.Object.vrstaUsluge+"' ,'"+ item.Object.vucnaSluzba + "') ");
            }
        }
    }
}