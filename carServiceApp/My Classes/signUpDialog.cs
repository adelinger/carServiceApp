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

namespace carServiceApp.My_Classes
{

    public class onSignUpEventArgs : EventArgs
    {
        public string firstName { get; set; }
        public string lastName  { get; set; }
        public string email     { get; set; }
        public string phone     { get; set; }
        public string password  { get; set; }

        public onSignUpEventArgs(string FirstName, string LastName, string Email, string Phone, string Password) :base()
        {
            firstName = FirstName;
            lastName = LastName;
            email = Email;
            phone = Phone;
            password = Password;
        }
    }
   public class signUpDialog :DialogFragment
    {
        private AutoCompleteTextView firstName;
        private AutoCompleteTextView lastName;
        private AutoCompleteTextView email;
        private AutoCompleteTextView password;
        private AutoCompleteTextView password2;
        private AutoCompleteTextView phone;
        private Button register;
        private bool termsAccepted;
        private bool close = false;

        public event EventHandler<onSignUpEventArgs> onSignUpComplete;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.signUpDialog, container, false);

            firstName = view.FindViewById<AutoCompleteTextView>(Resource.Id.ACTVime);
            lastName  = view.FindViewById<AutoCompleteTextView>(Resource.Id.ACTVprezime);
            email     = view.FindViewById<AutoCompleteTextView>(Resource.Id.ACTVemail);
            phone     = view.FindViewById<AutoCompleteTextView>(Resource.Id.ACTVphoneNumber);
            password  = view.FindViewById<AutoCompleteTextView>(Resource.Id.ACTVpassword);
            password2 = view.FindViewById<AutoCompleteTextView>(Resource.Id.ACTVpassword2);
            register  = view.FindViewById<Button>(Resource.Id.buttonSignUp);

            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            termsAgreementFragment agreementFragment = new termsAgreementFragment();
            agreementFragment.Show(transaction, "agreementFragment");

            agreementFragment.OnTermsAgreementChosen += AgreementFragment_OnTermsAgreementChosen;

            register.Click += Register_Click;

            return view;
        }

        private void Register_Click(object sender, EventArgs e)
        {
            if (email.Text == "" || password.Text == "" || firstName.Text == "" || lastName.Text == "" || phone.Text == "")
            {
                Toast.MakeText(View.Context, "Sva polja moraju biti ispunjena.", ToastLength.Long).Show();
                return;
            }
            if (password.Text != password2.Text)
            {
                Toast.MakeText(View.Context, "Lozinke se ne podudaraju", ToastLength.Long).Show();
                return;
            }
           

            if (!termsAccepted)
            {          
                this.Dismiss();

            }

            onSignUpComplete.Invoke(this, new onSignUpEventArgs(firstName.Text, lastName.Text, email.Text, phone.Text, password.Text));
            this.Dismiss();
        }   

        private void AgreementFragment_OnTermsAgreementChosen(object sender, onTermsAgreementChosenArgs e)
        {
            termsAccepted = e.termsAccepted;
            close = e.closeDIalog;

            if (!e.termsAccepted) { this.Dismiss(); }
            if(e.closeDIalog) { this.Dismiss(); }
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
           
        }
    }
}