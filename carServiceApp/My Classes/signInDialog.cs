using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using carServiceApp.Activities;
using Firebase;
using Firebase.Auth;

namespace carServiceApp.My_Classes
{
    class onSignInEventArgs
    {
        public string email   { get; set; }
        public string password{ get; set; }
        public bool zapamtiMe { get; set; }

        public onSignInEventArgs(string Email, string Password, bool ZapamtiMe)
        {
            email = Email;
            password = Password;
            zapamtiMe = ZapamtiMe;
        }    

    }

    class signInDialog : DialogFragment, IOnCompleteListener
    {
        private AutoCompleteTextView email;
        private AutoCompleteTextView password;
        private CheckBox zapamtiMe;
        private Button buttonforgotPassword;
        private Button prijaviSe;
        private View view;

        private static bool Checked;

        FirebaseAuth auth;

        private string forgot_email;

        public event EventHandler<onSignInEventArgs> onSignInComplete;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.signInDialog, container, false);
            email     = view.FindViewById<AutoCompleteTextView>(Resource.Id.prijavaEmail);
            password  = view.FindViewById<AutoCompleteTextView>(Resource.Id.prijavaPassword);
            zapamtiMe = view.FindViewById<CheckBox>(Resource.Id.checkBoxZapamtiMe);
            prijaviSe = view.FindViewById<Button>(Resource.Id.prijavaPrijaviSe);
            buttonforgotPassword = view.FindViewById<Button>(Resource.Id.forgotPassword);
            view.SetBackgroundColor(Color.White);

            auth = FirebaseAuth.GetInstance(loginActivity.app);

            buttonforgotPassword.Click += ButtonForgotPassword_Click;
            prijaviSe.Click += PrijaviSe_Click;
            zapamtiMe.CheckedChange += ZapamtiMe_CheckedChange;

            return view;

        }

        private void ZapamtiMe_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            Checked = e.IsChecked;
        }

        private void ButtonForgotPassword_Click(object sender, EventArgs e)
        {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            forgotPassword forgotPassword = new forgotPassword();
            forgotPassword.Show(transaction, "dialog fragment");
            forgotPassword.OnCompletePasswordRequest += ForgotPassword_OnCompletePasswordRequest;
        }

        private void ForgotPassword_OnCompletePasswordRequest(object sender, onCompletePasswordRequestArgs e)
        {
            forgot_email = e.email;
            auth.SendPasswordResetEmail(forgot_email).AddOnCompleteListener(this);
        }

        private void PrijaviSe_Click(object sender, EventArgs e)
        {
            if (email.Text == "" || password.Text == "")
            {
                Toast.MakeText(view.Context, "Neispravan unos", ToastLength.Long).Show();
                return;
            }
            onSignInComplete.Invoke(this, new onSignInEventArgs(email.Text, password.Text, Checked));
            this.Dismiss();
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
        }

        public void OnComplete(Task task)
        {
            if (task.IsSuccessful)
            {
                Toast.MakeText(view.Context, "Link za ponovno postavljanje lozinke vam je poslan na email.", ToastLength.Long).Show();
               
            }
            else
            {
                Toast.MakeText(view.Context, "Neuspješan povratak lozinke", ToastLength.Short).Show();              
            }
        }
    }

    
}