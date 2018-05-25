using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Content;
using carServiceApp.Activities;
using Firebase.Auth;
using System.Collections.Generic;
using carServiceApp.My_Classes;
using Firebase.Database;
using Java.Util;
using System.Linq;
using System;
using Firebase.Xamarin.Database;
using static carServiceApp.MainActivity;
using System.Threading.Tasks;
using Firebase;
using Firebase.Xamarin.Database.Query;

namespace carServiceApp
{

    [Activity(Label = "Moj servis", Icon = "@drawable/ifsedan285810")]
    public class MainActivity : Activity
    {
        private Button dogovoriSastanak;
        private Button mojAuto;
        private Button mojiSastanci;
        public string userName;

        private static FirebaseApp app;
        private FirebaseAuth auth;
        private string id;
        private string userLastName;

        connection con = new connection();
        createAppointment createAppointment = new createAppointment();
     
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
           
            SetContentView(Resource.Layout.Main);
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            dogovoriSastanak = FindViewById<Button>(Resource.Id.dogovoriTermin);
            mojAuto          = FindViewById<Button>(Resource.Id.myCarButton);
            mojiSastanci = FindViewById<Button>(Resource.Id.myAppointments);

            auth = FirebaseAuth.GetInstance(loginActivity.app);
            getUserInfo();            
            
            if (userName != null)
            {
                this.Title = userName + " " + userLastName;
            }
            else
            {
               await getUserInfoOnline();
                this.Title = userName;
            }

            dogovoriSastanak.Click += DogovoriSastanak_Click;
            mojAuto.Click += MojAuto_Click;
            mojiSastanci.Click += MojiSastanci_Click;
           
        }

        private void MojiSastanci_Click(object sender, EventArgs e)
        {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            chooseAppointment showAppointment = new chooseAppointment();
            showAppointment.Show(transaction, "fragment manager");

        }

        private void MojAuto_Click(object sender, EventArgs e)
        {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            chooseCar chooseCar = new chooseCar();
            chooseCar.Show(transaction, "dialog fragment");
        }

        private void DogovoriSastanak_Click(object sender, System.EventArgs e)
        {          
            Intent intent = new Intent(this, typeof(createAppointment));      
            StartActivity(intent);
        }

        public void getUserInfo ()
        {
            FirebaseUser users = FirebaseAuth.GetInstance(loginActivity.app).CurrentUser;
            id = users.Uid;
            List<User> data = con.db.Query<User>("SELECT * FROM User WHERE uid = '" + id + "' ");
            foreach (var item in data)
            {
                userName = item.name;
                userLastName = item.lastName;
            }
        }
       public async Task getUserInfoOnline()
        {
            var firebase = new FirebaseClient(loginActivity.FirebaseURL);
            var data = await firebase.Child("users").Child(id).OnceAsync<Account>();
            foreach (var item in data)
            {
                userName = item.Object.name + " " + item.Object.lastName;
            }
        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.actionbar_main, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.logOut)
            {
                auth.SignOut();
                Intent intent = new Intent(this, typeof(loginActivity));
                StartActivity(intent);               
            }
            return base.OnOptionsItemSelected(item);
        }

        public override void OnBackPressed()
        {
            this.FinishAffinity();
        }


    }

  

}



