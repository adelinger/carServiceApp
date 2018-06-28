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
using carServiceApp.My_Classes.Database;
using Android.Net;
using Android.Graphics.Drawables;

namespace carServiceApp
{

    [Activity(Label = "Moj servis", Icon = "@drawable/ifsedan285810")]
    public class MainActivity : Activity, IDialogInterfaceOnDismissListener
    {
        private Button dogovoriSastanak;
        private Button mojAuto;
        private Button mojiSastanci;
        private Button notifications;
        public string userName;

        private static FirebaseApp app;
        private FirebaseAuth auth;
        private string id;
        private string userLastName;

        connection con = new connection();
        createAppointment createAppointment = new createAppointment();

        public string FirebaseURL { get; private set; }

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
           
            SetContentView(Resource.Layout.Main);
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            dogovoriSastanak = FindViewById<Button>(Resource.Id.dogovoriTermin);
            mojAuto          = FindViewById<Button>(Resource.Id.myCarButton);
            mojiSastanci     = FindViewById<Button>(Resource.Id.myAppointments);
            notifications    = FindViewById<Button>(Resource.Id.notificationsButton);


            Drawable img = GetDrawable(Resource.Drawable.Envelope);
            img.SetBounds(0, 0, 0, 60);
            notifications.SetCompoundDrawables(null, null, img, null);

            if (!IsOnline())
            {
                checkIfOnline(this, this, this);
                return;
            }
            
            auth = FirebaseAuth.GetInstance(loginActivity.app);
            chooseCar.updateCars();
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
            string tag = "Online";
            if (IsOnline()) tag =  "Online";
            if (!IsOnline()) tag = "Offline";
            showAppointment.Show(transaction, tag);

        }

        private void MojAuto_Click(object sender, EventArgs e)
        {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            chooseCar chooseCar = new chooseCar();
            string tag = "Online";
            if (IsOnline()) tag = "Online";
            if (!IsOnline()) tag = "Offline";
            chooseCar.Show(transaction, tag);
        }

        private void DogovoriSastanak_Click(object sender, System.EventArgs e)
        {          
            Intent intent = new Intent(this, typeof(createAppointment));      
            StartActivity(intent);
        }

        public bool IsOnline()
        {
            var cm = (ConnectivityManager)GetSystemService(ConnectivityService);
            return cm.ActiveNetworkInfo == null ? false : cm.ActiveNetworkInfo.IsConnected;
        }

        public static void checkIfOnline(Context context, Activity activity, IDialogInterfaceOnDismissListener listener)
        {
            AlertDialog.Builder dialogs = new AlertDialog.Builder(context);
            dialogs.SetOnDismissListener(listener);
            dialogs.SetMessage("Veza na internet je neophodna za nastavak. Pokušajte kasnije.");
            dialogs.SetPositiveButton("U redu", (senderAlert, args) => {
                activity.Finish();
                dialogs.Dispose();
            });
            Dialog alertDialogs = dialogs.Create();
            alertDialogs.Show();
            return;
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
            if (id == Resource.Id.support)
            {
                FragmentTransaction transaction = FragmentManager.BeginTransaction();
                aboutUs aboutUs = new aboutUs();
                aboutUs.Show(transaction, "aboutUs");
            }
            if (id == Resource.Id.personalInfo)
            {
                FragmentTransaction transaction = FragmentManager.BeginTransaction();
                personalInfo personalInfo = new personalInfo();
                personalInfo.Show(transaction, "personalInfo");
            }
            return base.OnOptionsItemSelected(item);
        }

        public override void OnBackPressed()
        {
            this.FinishAffinity();
        }

        protected override void OnResume()
        {
            if (IsOnline()) chooseCar.updateCars();
            base.OnResume();
        }

        public void OnDismiss(IDialogInterface dialog)
        {
            dialog.Dispose();
            this.Finish();
            Intent intent = new Intent(this, typeof(loginActivity));
            StartActivity(intent);
        }
    }

  

}



