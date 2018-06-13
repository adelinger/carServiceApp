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
using carServiceApp.My_Classes;
using carServiceApp.My_Classes.Database;
using Firebase.Auth;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;
using Newtonsoft.Json;

namespace carServiceApp.Activities
{
    [Activity(Label = "Potvrdite sastanak")]
    public class confirmOrder : Activity, IDialogInterfaceOnDismissListener
    {

        private Button   confirmOrderButton;
        private Button   addDate;
        private TextView userInfo;
        private TextView chosenServices;
        private TextView chosenCar;
        private TextView opisKvara;
        private TextView addedDate;

        private string getOpisKvara;
        private string vrstaPosla;
        private string vrstaUsluge;
        private string carChosen;
        private string zahtjevZaVucnomSluzbom;
        private string zahtjevZaNarucivanjem;
        private bool   potrebnaVucnaSluzba;
        private bool   potrebnoNarucivanje;

        private string id;
        private string userData;
        private string orderID;

        connection con = new connection();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.confirmOrder);
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            confirmOrderButton = FindViewById<Button>(Resource.Id.COconfirmOrderButton);
            userInfo           = FindViewById<TextView>(Resource.Id.COuserInfo);
            chosenServices     = FindViewById<TextView>(Resource.Id.COchosenServices);
            chosenCar          = FindViewById<TextView>(Resource.Id.COchosenCar);
            opisKvara          = FindViewById<TextView>(Resource.Id.COopisKvara);
            addDate            = FindViewById<Button>(Resource.Id.COpickaDate);
            addedDate          = FindViewById<TextView>(Resource.Id.bestPossibleDate);

            vrstaPosla          = Intent.GetStringExtra("vrstaPosla");
            vrstaUsluge         = Intent.GetStringExtra("vrstaUsluge");
            carChosen           = Intent.GetStringExtra("carChosen");
            getOpisKvara        = Intent.GetStringExtra("opisKvara");
            potrebnaVucnaSluzba = Intent.GetBooleanExtra("potrebnaVucnaSluzba", false);
            potrebnoNarucivanje = Intent.GetBooleanExtra("potrebnoNarucivanje", false);

            if (potrebnaVucnaSluzba)  zahtjevZaVucnomSluzbom = "Da";
            if (!potrebnaVucnaSluzba) zahtjevZaVucnomSluzbom = "Ne";
            if (potrebnoNarucivanje)  zahtjevZaNarucivanjem  = "Da";
            if (!potrebnoNarucivanje) zahtjevZaNarucivanjem  = "Ne";

            var auth = FirebaseAuth.GetInstance(loginActivity.app);
            id = auth.CurrentUser.Uid;

            getUserInfo();

            userInfo.Text       = userData;
            chosenServices.Text = vrstaUsluge + ", " + vrstaPosla + ", " + "Zahtjev za vučnom službom: " + "" + zahtjevZaVucnomSluzbom + ", " + "Zahtjev za naručivanjem dijelova: " + zahtjevZaNarucivanjem;
            chosenCar.Text      = carChosen;
            opisKvara.Text      = getOpisKvara;

            userInfo.Click       += UserInfo_Click;
            chosenCar.Click      += ChosenCar_Click;
            opisKvara.Click      += OpisKvara_Click;
            chosenServices.Click += ChosenServices_Click;

            confirmOrderButton.Click += ConfirmOrderButton_Click;
            addDate.Click += AddDate_Click;
        }

        private void AddDate_Click(object sender, EventArgs e)
        {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            calendarView calendar = new calendarView();
            calendar.Show(transaction, "fragment fragment");
            calendar.onDatePickedEvent += Calendar_onDatePickedEvent;
        }

        private void Calendar_onDatePickedEvent(object sender, onDatePickedEventArgs e)
        {
            addedDate.Text = e.datePicked;
        }

        public bool IsOnline()
        {
            var cm = (ConnectivityManager)GetSystemService(ConnectivityService);
            return cm.ActiveNetworkInfo == null ? false : cm.ActiveNetworkInfo.IsConnected;
        }

        
        private  async void ConfirmOrderButton_Click(object sender, EventArgs e)
        {
           if (!IsOnline())
            {
                MainActivity.checkIfOnline(this, this, this);
                return;
            }

            var auth = FirebaseAuth.GetInstance(loginActivity.app);
            id = auth.CurrentUser.Uid;
            var firebase = new FirebaseClient(loginActivity.FirebaseURL);


            orders orders = new orders();
            orders.uid               = id;
            orders.carName           = carChosen;
            orders.vrstaUsluge       = vrstaUsluge;
            orders.vrstaPosla        = vrstaPosla;
            orders.opisKvara         = opisKvara.Text;
            orders.datum             = DateTime.Now.ToLocalTime().ToString();
            orders.vucnaSluzba       = potrebnaVucnaSluzba;
            orders.dijelovi          = potrebnoNarucivanje;
            orders.pozeljniDatum     = addedDate.Text;
            orders.vrijemeServisa    = "";
            orders.cijena            = "";
            orders.napomenaServisera = "";

            int numOfOrders = 1;
            orderID = JsonConvert.SerializeObject(numOfOrders.ToString());
            
            List<orders> allOrders = new List<orders>();                
            var getONumberOfOrders = await firebase.Child("order").Child(id).OnceAsync<orders>();
            foreach (var item in getONumberOfOrders)
            {
                allOrders.Add(new orders { carName = item.Object.carName });
                numOfOrders = allOrders.Count +1;               
                orderID = numOfOrders.ToString();
                orderID = JsonConvert.SerializeObject(orderID);
            }

            orders.id = numOfOrders.ToString();

            try
            {
                var addOrder = firebase.Child("order").Child(id).Child(orderID.ToString()).PutAsync<orders>(orders);
                con.db.Insert(orders);
            }
            catch (Exception)
            {
                throw;
            }
           

            AlertDialog.Builder dialog = new AlertDialog.Builder(this);
            dialog.SetOnDismissListener(this);
            dialog.SetMessage("Uspješno ste poslali zahtjev za popravkom. Status zahtjeva možete pratiti pod 'Moji sastanci' u glavnom izborniku");
            dialog.SetPositiveButton("U redu", (senderAlert, args) => {
                dialog.Dispose();
                this.Finish();
                addCarToOrder.finish.Finish();
                Intent intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            });
            Dialog alertDialog = dialog.Create();
            alertDialog.Show();
           
        }

        private void ChosenServices_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(createAppointment)).SetFlags(ActivityFlags.ReorderToFront);
            buildDialog(intent);
        }

        private void OpisKvara_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(addCarToOrder)).SetFlags(ActivityFlags.ReorderToFront);
            buildDialog(intent);
        }

        private void ChosenCar_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(addCarToOrder)).SetFlags(ActivityFlags.ReorderToFront);
            buildDialog(intent);
        }

        private void UserInfo_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(createAppointment)).SetFlags(ActivityFlags.ReorderToFront);
            buildDialog(intent);
        }

        private void buildDialog(Intent intent)
        {
            AlertDialog.Builder dialog = new AlertDialog.Builder(this);
            dialog.SetMessage("Želite li izmijeniti ovaj unos?");
            dialog.SetPositiveButton("Izmijeni", (senderAlert, args) => {           
                StartActivity(intent);
            });
            dialog.SetNegativeButton("Odustani", (senderAlert, args) => {
                dialog.Dispose();
            });
            Dialog alertDialog = dialog.Create();
            alertDialog.Show();
           
        }


        private void getUserInfo()
        {
            List<User> getUserData = con.db.Query<User>("SELECT * FROM User WHERE uid = '" + id + "' ");
            foreach (var item in getUserData)
            {
                userData = item.name + item.lastName + ", " + item.adress + " " + item.city + ", " + item.phone + ", " + item.email;
            }
        }

        public void OnDismiss(IDialogInterface dialog)
        {
            if (IsOnline())
            {
                dialog.Dispose();
                this.Finish();
                addCarToOrder.finish.Finish();
                Intent intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            }
            else
            {
                dialog.Dispose();
                this.Finish();
                Intent intent = new Intent(this, typeof(loginActivity));
                StartActivity(intent);
            }
        }
    }
}