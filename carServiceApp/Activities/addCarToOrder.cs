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
    [Activity(Label = "Dodajte vozilo")]
    public class addCarToOrder : Activity
    {
        private Spinner     spinner;
        private Button      addNewCar;
        private Button      next;
        private EditText    opisiteProblem;
        private RadioButton yesButton;
        private RadioButton noButton;
        private RadioButton imamDijelove;
        private RadioButton naruciDijelove;

        private bool potrebnaVucnaSluzba = false;
        private bool potrebnoNarucivanje = false;
        private bool updateRequested;

        private string vrstaUsluge;
        private string vrstaPosla;
        private string id;
        private List<string> carList = new List<string>();

        connection con = new connection();
        public static Activity finish;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.addCarToOrder);
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            addNewCar      = FindViewById<Button>(Resource.Id.ACTOaddNewCar);
            next           = FindViewById<Button>(Resource.Id.ACTOnext);
            opisiteProblem = FindViewById<EditText>(Resource.Id.opisiteKvarEditText);
            spinner        = FindViewById<Spinner>(Resource.Id.spinnerUserCars);
            yesButton      = FindViewById<RadioButton>(Resource.Id.yesButton);
            noButton       = FindViewById<RadioButton>(Resource.Id.noButton);
            imamDijelove   = FindViewById<RadioButton>(Resource.Id.imamDijeloveButton);
            naruciDijelove = FindViewById<RadioButton>(Resource.Id.zelimNarucitiButton);

            finish = this;

            vrstaPosla      = Intent.GetStringExtra("vrstaPosla");
            vrstaUsluge     = Intent.GetStringExtra("vrstaUsluge");

            loadSpinner();

            next.Click      += Next_Click;
            addNewCar.Click += AddNewCar_Click;
        }

        private void checkIfChecked()
        {
            if (yesButton.Checked)      potrebnaVucnaSluzba = true;
            if (noButton.Checked)       potrebnaVucnaSluzba = false;
            if (imamDijelove.Checked)   potrebnoNarucivanje = false;
            if (naruciDijelove.Checked) potrebnoNarucivanje = true;
        }

        private void Next_Click(object sender, EventArgs e)
        {
            if (spinner.SelectedItem.ToString() == "Odaberite stavku")
            {
                Toast.MakeText(this, "Morate odabrati vozilo da biste mogli nastaviti", ToastLength.Long).Show();
                return;
            }
            if(opisiteProblem.Text == "")
            {
                Toast.MakeText(this, "Morate prvo ukratko opisati kvar da biste mogli nastaviti", ToastLength.Long).Show();
                return;
            }

            checkIfChecked();
            
            if (updateRequested)
            {
                Intent intent = new Intent(this, typeof(confirmOrder)).SetFlags(ActivityFlags.ReorderToFront);
                intent.PutExtra("vrstaPosla", vrstaPosla);
                intent.PutExtra("vrstaUsluge", vrstaUsluge);
                intent.PutExtra("carChosen", spinner.SelectedItem.ToString());
                intent.PutExtra("potrebnaVucnaSluzba", potrebnaVucnaSluzba);
                intent.PutExtra("opisKvara", opisiteProblem.Text);
                intent.PutExtra("potrebnoNarucivanje", potrebnoNarucivanje);
                StartActivity(intent);
            }
            else
            {
                Intent intent = new Intent(this, typeof(confirmOrder));
                intent.PutExtra("vrstaPosla", vrstaPosla);
                intent.PutExtra("vrstaUsluge", vrstaUsluge);
                intent.PutExtra("carChosen", spinner.SelectedItem.ToString());
                intent.PutExtra("potrebnaVucnaSluzba", potrebnaVucnaSluzba);
                intent.PutExtra("opisKvara", opisiteProblem.Text);
                intent.PutExtra("potrebnoNarucivanje", potrebnoNarucivanje);
                StartActivity(intent);
            }

        }

        protected override void OnResume()
        {
            loadSpinner();
            updateRequested = true;
            base.OnResume();
        }

        private void AddNewCar_Click(object sender, EventArgs e)
        {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            addCar addCar = new addCar();
            addCar.Show(transaction, "addCar");
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