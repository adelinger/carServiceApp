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

namespace carServiceApp
{

    [Activity(Label = "Moj servis", Icon = "@drawable/ifsedan285810")]
    public class MainActivity : Activity
    {
        private Button dogovoriSastanak;
        public string userName;

        private static FirebaseApp app;
        private FirebaseAuth auth;
        private string id;
     
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
           
            SetContentView(Resource.Layout.Main);
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            dogovoriSastanak = FindViewById<Button>(Resource.Id.dogovoriTermin);

            auth = FirebaseAuth.GetInstance(loginActivity.app);

            dogovoriSastanak.Click += DogovoriSastanak_Click;

           
        }

        private void DogovoriSastanak_Click(object sender, System.EventArgs e)
        {          
            Intent intent = new Intent(this, typeof(createAppointment));      
            StartActivity(intent);
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



