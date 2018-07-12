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
using Firebase.Iid;
using Firebase.Messaging;
using carServiceApp.My_Classes.myFirebaseMessaging;
using Android.Support.V4.Content;

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

        private bool messageReceived = false;
        MyMessageReceiver myReceiver;

        connection con = new connection();
        createAppointment createAppointment = new createAppointment();

        public string FirebaseURL { get; private set; }

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            myReceiver = new MyMessageReceiver();

            SetContentView(Resource.Layout.Main);
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            dogovoriSastanak = FindViewById<Button>(Resource.Id.dogovoriTermin);
            mojAuto          = FindViewById<Button>(Resource.Id.myCarButton);
            mojiSastanci     = FindViewById<Button>(Resource.Id.myAppointments);
            notifications    = FindViewById<Button>(Resource.Id.notificationsButton);


            FirebaseDatabase.GetInstance(loginActivity.FirebaseURL).SetPersistenceEnabled(true);  

            string authorizedEntity = "carserviceapp-5132f";
            string scope = "GCM";

 
             #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(() => {
              var instanceId = FirebaseInstanceId.Instance;
              instanceId.DeleteInstanceId();
              string token = instanceId.GetToken(authorizedEntity, scope);
              
              Android.Util.Log.Debug("TAG", "{0} {1}", instanceId.Token, token, Firebase.Messaging.FirebaseMessaging.InstanceIdScope);

            });
            #pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            messageReceived = Intent.GetBooleanExtra("messageReceived", false);
            if (messageReceived)
            {
                Toast.MakeText(this, "Imate novu poruku heheheheh", ToastLength.Long).Show();
            }


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
            notifications.Click += Notifications_Click;
 
        }

        private void Notifications_Click(object sender, EventArgs e)
        {

        }

      

        private void MojiSastanci_Click(object sender, EventArgs e)
        {
            if (!IsOnline())
            {
                Toast.MakeText(this, "Nije moguće provjeriti sastanke ako niste povezani na internet", ToastLength.Long).Show();
                return;
            }
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
            MyFirebaseMessagingService myMessaging = new MyFirebaseMessagingService();
            myMessaging.OnMessageReceivedEvent += MyMessaging_OnMessageReceivedEvent;

            if (IsOnline()) chooseCar.updateCars();
            base.OnResume();
    
            LocalBroadcastManager.GetInstance(this).RegisterReceiver(myReceiver, new IntentFilter("message"));
            RegisterReceiver(myReceiver, new IntentFilter("message"));
        }

        private void MyMessaging_OnMessageReceivedEvent(object sender, OnMessageReceivedArgs e)
        {
            
        }

        protected override void OnPause()
        {
            base.OnPause();
            LocalBroadcastManager.GetInstance(this).UnregisterReceiver(myReceiver);
        }

        public void OnDismiss(IDialogInterface dialog)
        {
            dialog.Dispose();
            this.Finish();
            Intent intent = new Intent(this, typeof(loginActivity));
            StartActivity(intent);
        }
    }


    [BroadcastReceiver(Enabled = true, Exported = false)]
    public class MyMessageReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            bool messageReceived = intent.GetBooleanExtra("messageReceived", false);
        }
    }

  

}



