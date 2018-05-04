using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using carServiceApp.My_Classes;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using Java.Lang;
using static Android.Views.View;
using Firebase.Xamarin.Database;
using static Java.Util.Locale;
using Firebase.Xamarin.Database.Query;

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

        public static FirebaseApp app;
        private string id;
        public const string FirebaseURL = "https://carserviceapp-5132f.firebaseio.com/";
        FirebaseAuth auth;

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

            rememberIfIsChecked();


            mButtuonSignUp.Click += MButtuonSignUp_Click;
            buttonSignIn.Click += ButtonSignIn_Click;

            FirebaseUser user = FirebaseAuth.GetInstance(app).CurrentUser;
            if (login_rememberMe)
            {
                if (user != null)
                {
                    Intent intent = new Intent(this, typeof(MainActivity));
                    StartActivity(intent);
                }
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
            updateUser(login_rememberMe);
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
            Account user = new Account();
            user.uid = id;
            user.name = signup_inputName;
            user.lastName = signup_inputLastName;
            user.Email = signup_inputEmail;
            user.phone = signup_inputPhoneNumber;
            user.rememberMe = false;

            var firebase = new FirebaseClient(FirebaseURL);

            var item = firebase.Child("users").Child(id).PutAsync<Account>(user);
          
        }
        private async void updateUser (bool rememberMe)
        {
            var firebase = new FirebaseClient(FirebaseURL);
            await firebase.Child("users").Child(id).Child("rememberMe").PutAsync(rememberMe);

        }
        private async void rememberIfIsChecked ()
        {
            var firebase = new FirebaseClient(FirebaseURL);
            FirebaseUser thisUser = FirebaseAuth.GetInstance(app).CurrentUser;
            id = thisUser.Uid;
            var items = await firebase.Child("users").OnceAsync<Account>();

            foreach (var item in items)
            {
                Account user = new Account();
               
               user.rememberMe = item.Object.rememberMe;
               login_rememberMe = user.rememberMe;
            }
        }


        public void OnSuccess(Java.Lang.Object result)
        {
            Intent intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            Toast.MakeText(this, "Netočan email/password!", ToastLength.Short).Show();
            progressBar.Visibility = ViewStates.Invisible;
        }

        public override void OnBackPressed()
        {
            this.FinishAffinity();
        }
    }
}