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
using carServiceApp.My_Classes;
using carServiceApp.My_Classes.Database;
using Firebase.Auth;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;

namespace carServiceApp.Activities
{
    [Activity(Label = "confirmOrder")]
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
        }

        private void ConfirmOrderButton_Click(object sender, EventArgs e)
        {
            var auth = FirebaseAuth.GetInstance(loginActivity.app);
            id = auth.CurrentUser.Uid;
            var firebase = new FirebaseClient(loginActivity.FirebaseURL);

            order order       = new order();
            order.uid         = id;
            order.carName     = carChosen;
            order.vrstaUsluge = vrstaUsluge;
            order.vrstaPosla  = vrstaPosla;
            order.opisKvara   = opisKvara.Text;
            order.datum       = DateTime.UtcNow;
            order.vucnaSluzba = potrebnaVucnaSluzba;
            order.dijelovi    = potrebnoNarucivanje;

            con.db.Insert(order);

            try
            {
                var addOrder = firebase.Child("order").Child(id).Child(order.id.ToString()).PostAsync<order>(order);
            }
            catch (Exception)
            {
                throw;
            }
            Toast.MakeText(this, "Uspješno ste poslali zahtjev za popravkom. Status zahtjeva možete pratiti pod 'Moji sastanci' u glavnom izborniku", ToastLength.Long).Show();

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
            dialog.Dispose();
            this.Finish();
            addCarToOrder.finish.Finish();
            Intent intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
        }
    }
}