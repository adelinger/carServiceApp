using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using carServiceApp.My_Classes;
using Firebase;
using Firebase.Auth;
using static Android.Views.View;

namespace carServiceApp.Activities
{
    [Activity(Label = "Moj servis", MainLauncher = true)]
    public class loginActivity : Activity, IOnClickListener, IOnCompleteListener
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

        public static FirebaseApp app;
        FirebaseAuth auth;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.login);

           // mButtuonSignUp = FindViewById<Button>(Resource.Id);
            //buttonSignIn = FindViewById<Button>(Resource.Id.loginButton);
            //  progressBar    = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            activityLogin = FindViewById<LinearLayout>(Resource.Layout.login);

            InitFirebaseAuth();

            mButtuonSignUp.Click += MButtuonSignUp_Click;
            buttonSignIn.Click += ButtonSignIn_Click;
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

            auth.SignInWithEmailAndPassword(login_inputMail, login_inputPassword);

            //progressBar.Visibility = ViewStates.Visible;
            //Thread thread = new Thread(actLikeThread);
            //thread.Start();
        }

        private void MButtuonSignUp_Click(object sender, EventArgs e)
        {
            //pull up dialog
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            signUpDialog dialog = new signUpDialog();
            dialog.Show(transaction, "dialog fragment");
            dialog.onSignUpComplete += Dialog_onSignUpComplete;
        }

        private void Dialog_onSignUpComplete(object sender, onSignUpEventArgs e)
        {
            signup_inputName = e.firstName;
            signup_inputLastName = e.lastName;
            signup_inputPhoneNumber = e.phone;
            signup_inputEmail = e.email;
            signup_inputPassword = e.password;

            OnComplete(auth.CreateUserWithEmailAndPassword(signup_inputEmail, signup_inputPassword));
          
            progressBar.Visibility = ViewStates.Visible;
            Thread thread = new Thread(actLikeThread);
            thread.Start();
        }

        public void actLikeThread ()
        {
            Thread.Sleep(4000);
            //  RunOnUiThread(() => { progressBar.Visibility = ViewStates.Invisible; });

            RunOnUiThread(test);
        }
        public void test()
        {
            progressBar.Visibility = ViewStates.Invisible;
        }

        public void OnClick(View v)
        {
            
        }

        public void OnComplete(Task task)
        {
            if (task.IsSuccessful)
            {
                Snackbar snack = Snackbar.Make(activityLogin, "Uspješno ste se registrirali", Snackbar.LengthShort);
                snack.Show();
                Toast.MakeText(this, "Uspješno", ToastLength.Short).Show();
            }
        }
    }
}