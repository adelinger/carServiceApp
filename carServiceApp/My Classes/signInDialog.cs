using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

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

    class signInDialog : DialogFragment
    {
        private AutoCompleteTextView email;
        private AutoCompleteTextView password;
        private CheckBox zapamtiMe;
        private Button forgotPassword;
        private Button prijaviSe;

        public event EventHandler<onSignInEventArgs> onSignInComplete;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.signInDialog, container, false);
            email     = view.FindViewById<AutoCompleteTextView>(Resource.Id.prijavaEmail);
            password  = view.FindViewById<AutoCompleteTextView>(Resource.Id.prijavaPassword);
            zapamtiMe = view.FindViewById<CheckBox>(Resource.Id.checkBoxZapamtiMe);
            prijaviSe = view.FindViewById<Button>(Resource.Id.prijavaPrijaviSe);
            forgotPassword = view.FindViewById<Button>(Resource.Id.forgotPassword);
            view.SetBackgroundColor(Color.White);

            prijaviSe.Click += PrijaviSe_Click;

            return view;
        }

        private void PrijaviSe_Click(object sender, EventArgs e)
        {
            onSignInComplete.Invoke(this, new onSignInEventArgs(email.Text, password.Text, zapamtiMe.Checked));
            this.Dismiss();
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
        }

    }
}