using System;
using System.Collections.Generic;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.Net;
using Android.OS;
using Android.Views;
using Android.Widget;
using carServiceApp.My_Classes;
using carServiceApp.My_Classes.Database;
using Firebase;
using Firebase.Auth;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;
using Newtonsoft.Json;

namespace carServiceApp.Activities
{
    [Activity(Label = "Moj servis", MainLauncher = true)]
    public class loginActivity : Activity, IOnCompleteListener, IOnSuccessListener, IOnFailureListener
    {
        private Button mButtuonSignUp;
        private Button buttonSignIn;
        private ProgressBar progressBar;

        private string signup_inputName;
        private string signup_inputLastName;
        private string signup_inputEmail;
        private string signup_inputPhoneNumber;
        private string signup_inputPassword;
        private LinearLayout activityLogin;

        private string login_inputMail;
        private string login_inputPassword;
        private static bool login_rememberMe;

        public List<string> listOfServices = new List<string>();
        public string name;

        public static FirebaseApp app;
        private string id;
        private string key;
        public const string FirebaseURL = "https://carserviceapp-5132f.firebaseio.com/";
        FirebaseAuth auth;

        createAppointment createAppointment = new createAppointment();
        connection con = new connection();
      

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.logInLayout);
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            

            mButtuonSignUp = FindViewById<Button>(Resource.Id.registerButton);
            buttonSignIn   = FindViewById<Button>(Resource.Id.Loginbutton);
            progressBar    = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            activityLogin  = FindViewById<LinearLayout>(Resource.Layout.logInLayout);

            progressBar.Visibility = ViewStates.Invisible;
            
            InitFirebaseAuth();
            updateServices();

            con.db.CreateTable<User>();
            con.db.CreateTable<carDetailsSQL>();
            con.db.CreateTable<orders>();

            mButtuonSignUp.Click += MButtuonSignUp_Click;
            buttonSignIn.Click += ButtonSignIn_Click;

            FirebaseUser user = FirebaseAuth.GetInstance(app).CurrentUser;

            if (user != null)
            {
                checkIfRememberMeIsChecked();                
            }
            if (login_rememberMe && user != null && IsOnline())
            {
                createAppointment.updateUser();
                updateCars();
                Intent intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            }
           
        }

        public bool IsOnline()
        {
            var cm = (ConnectivityManager)GetSystemService(ConnectivityService);
            return cm.ActiveNetworkInfo == null ? false : cm.ActiveNetworkInfo.IsConnected;
        }

        public async void updateCars()
        {
            List<carDetailsSQL> cars = new List<carDetailsSQL>();
            var firebase = new FirebaseClient(loginActivity.FirebaseURL);

            var data = await firebase.Child("car").Child(id).OnceAsync<CarDetails>();
            foreach (var item in data)
            {
               
                con.db.Execute("INSERT OR IGNORE INTO carDetailsSQL (carName, godina, markaVozila, modelVozila, snagaMotora, tipMotora, tipVozila, uid, zapremninaMotora) VALUES ('" + item.Key + "', " +
                    " '" + item.Object.godina + "', '" + item.Object.markaVozila + "', '" + item.Object.modelVozila + "', '" + item.Object.snagaMotora + "', '" + item.Object.tipMotora + "', '" + item.Object.tipVozila + "', " +
                    "'" + item.Object.uid + "', '" + item.Object.zapremninaMotora + "') ");

            }


        }

        private void InitFirebaseAuth()
        {
            var options = new FirebaseOptions.Builder().SetApplicationId("1:909560725502:android:4f4c5f3cd00da486").SetApiKey("AIzaSyDJro0RffLOLQ_xtKOXOoreLHzZoQ7M5y0").Build();
            if (app == null)
                app = FirebaseApp.InitializeApp(this, options);
            auth = FirebaseAuth.GetInstance(app);
        }

        private void ButtonSignIn_Click(object sender, EventArgs e)
        {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            signInDialog dialog = new signInDialog();
            dialog.Show(transaction, "dialog fragment");
            dialog.onSignInComplete += Dialog_onSignInComplete;
         
        }

        private void Dialog_onSignInComplete(object sender, onSignInEventArgs e)
        {       
            login_inputMail = e.email;
            login_inputPassword = e.password;
            login_rememberMe = e.zapamtiMe;

            auth.SignInWithEmailAndPassword(login_inputMail, login_inputPassword).AddOnSuccessListener(this, this).AddOnFailureListener(this, this);          
            progressBar.Visibility = ViewStates.Visible;
        }

        private void MButtuonSignUp_Click(object sender, EventArgs e)
        {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            signUpDialog dialog = new signUpDialog();
            dialog.Show(transaction, "dialog fragment");
            dialog.onSignUpComplete += Dialog_onSignUpComplete;
        }

        private void Dialog_onSignUpComplete(object sender, onSignUpEventArgs e)
        {      
            signup_inputName        = e.firstName;
            signup_inputLastName    = e.lastName;
            signup_inputPhoneNumber = e.phone;
            signup_inputEmail       = e.email;
            signup_inputPassword    = e.password;

           auth.CreateUserWithEmailAndPassword(signup_inputEmail, signup_inputPassword).AddOnCompleteListener(this, this);
   
           progressBar.Visibility = ViewStates.Visible;

        }

        public void OnComplete(Task task)
        {
            if (task.IsSuccessful)
            {
                Toast.MakeText(this, "Uspješno ste se registrirali. Možete se prijaviti.", ToastLength.Long).Show();

                FirebaseUser user = FirebaseAuth.GetInstance(app).CurrentUser;
                id = user.Uid;
                 

                CreateUser();

                buttonSignIn.PerformClick();
                progressBar.Visibility = ViewStates.Invisible;
            }
            else
            {
                Toast.MakeText(this, "Neuspješna registracija. Možda postoji problem sa vezom ili korisnik sa dodanim emailom već postoji. ", ToastLength.Long).Show();
                progressBar.Visibility = ViewStates.Invisible;
            }
        }

        private async void CreateUser ()
        {
            User OfflineUser = new User();
            OfflineUser.uid      = id;
            OfflineUser.name     = signup_inputName;
            OfflineUser.lastName = signup_inputLastName;
            OfflineUser.email    = signup_inputEmail;
            OfflineUser.phone    = signup_inputPhoneNumber;

            Account user = new Account();
            user.uid      = id;
            user.name     = signup_inputName;
            user.lastName = signup_inputLastName;
            user.email    = signup_inputEmail;
            user.phone    = signup_inputPhoneNumber;
            user.city     = "";
            user.adress   = "";

            con.db.CreateTable<User>(); //creating table in offline database
            con.db.Insert(OfflineUser);  //inserting into offline database

            var firebase = new FirebaseClient(FirebaseURL); 

            var item = firebase.Child("users").Child(id).PostAsync<Account>(user);  //inserting into online database
          
        }  

        private async void updateServices ()
        {
            var firebase = new FirebaseClient(FirebaseURL);
            var items = await firebase.Child("services").OnceAsync<services>();

            var itemsAMU  = await firebase.Child("usluge").Child("Automehaničarske usluge").OnceAsync<usluge>();
            var itemsALU  = await firebase.Child("usluge").Child("Autolakirerske usluge").  OnceAsync<usluge>();
            var itemsALIU = await firebase.Child("usluge").Child("Autolimarske usluge").    OnceAsync<usluge>();
            var itemsVUl  = await firebase.Child("usluge").Child("Vulkanizerske usluge").   OnceAsync<usluge>();
            var itemsOU   = await firebase.Child("usluge").Child("Ostale usluge").          OnceAsync<usluge>();

            con.db.CreateTable<services>();
            con.db.CreateTable<uslugeSQL>();

            foreach (var item in items)
            {
                con.db.Execute("INSERT OR IGNORE INTO services (name) VALUES ( '" + item.Object.name + "') ");            
            }
            foreach (var item in itemsAMU)
            {
                con.db.Execute("INSERT OR IGNORE INTO uslugeSQL (name, type) VALUES ( '" + item.Object.name + "', 'Automehaničarske usluge') ");
            }
            foreach (var item in itemsALU)
            {
                con.db.Execute("INSERT OR IGNORE INTO uslugeSQL (name, type) VALUES ( '" + item.Object.name + "', 'Autolakirerske usluge') ");
            }
            foreach (var item in itemsALIU)
            {
                con.db.Execute("INSERT OR IGNORE INTO uslugeSQL (name, type) VALUES ( '" + item.Object.name + "', 'Autolimarske usluge') ");
            }
            foreach (var item in itemsVUl)
            {
                con.db.Execute("INSERT OR IGNORE INTO uslugeSQL (name, type) VALUES ( '" + item.Object.name + "', 'Vulkanizerske usluge') ");
            }
            foreach (var item in itemsOU)
            {
                con.db.Execute("INSERT OR IGNORE INTO uslugeSQL (name, type) VALUES ( '" + item.Object.name + "', 'Ostale usluge') ");
            }
            
        }

        private async void updateUser(bool rememberMe)
        {
            int boolValue = 0;
            string uid = "";
            if (rememberMe == true) boolValue = 1;
            User user = new User();

            var firebase = new FirebaseClient(loginActivity.FirebaseURL);
            FirebaseUser CurrentUser = FirebaseAuth.GetInstance(app).CurrentUser;
            id = CurrentUser.Uid;

            List<User> getUser = con.db.Query<User>("SELECT * FROM User WHERE uid = '" + id + "' ");
            foreach (var item in getUser)
            {
                uid = item.uid;
            } 

            var data = await firebase.Child("users").Child(id).OnceAsync<Account>();
            foreach (var item in data)
            {
                user.name = item.Object.name;
                user.lastName = item.Object.lastName;
                user.phone = item.Object.phone;
                user.email = item.Object.email;
                user.city = item.Object.city;
                user.adress = item.Object.adress;
            }

            if (uid == "")
            {
                User newUser = new User();
                newUser.name     = user.name;
                newUser.lastName = user.lastName;
                newUser.phone    = user.phone;
                newUser.email    = user.email;
                newUser.city     = user.city;
                newUser.adress   = user.adress;
                newUser.uid      = id;
                con.db.Insert(newUser);
            }
            
            con.db.Execute("UPDATE User SET rememberMe = '" + boolValue + "' WHERE uid = '" + id + "' ");
                       
            
            con.db.Execute("UPDATE User SET name = '"+user.name+"', lastName = '"+user.lastName+"', phone = '"+user.phone+"', " +
                "email = '"+user.email+"', city = '"+user.city+"', adress = '"+user.adress+"' WHERE uid = '"+id+"' ");

            
        }

       

        private void checkIfRememberMeIsChecked ()
        {
            FirebaseUser user = FirebaseAuth.GetInstance(app).CurrentUser;
            id = user.Uid;
            List<User> getIfChecked = con.db.Query<User>("SELECT * FROM User WHERE uid = '"+id+"' ");
            foreach (var item in getIfChecked)
            {
                login_rememberMe = item.rememberMe;
            }
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            updateUser(login_rememberMe);
            Intent intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            Toast.MakeText(this, "Netočan email/password!", ToastLength.Short).Show();
            buttonSignIn.PerformClick();
            progressBar.Visibility = ViewStates.Invisible;
        }

        public override void OnBackPressed()
        {
            this.FinishAffinity();
        }
    }
}