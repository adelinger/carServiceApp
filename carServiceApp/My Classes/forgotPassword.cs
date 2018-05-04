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

    public class onCompletePasswordRequestArgs
    {
        public string email { get; set; }

        public onCompletePasswordRequestArgs(string Email)
        {
            email = Email;
        }
    }

    class forgotPassword :DialogFragment
    {
        private AutoCompleteTextView forgotPassword_userInput;
        private Button sendRequest;

        public event EventHandler<onCompletePasswordRequestArgs> OnCompletePasswordRequest;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.forgotPasswordLayout, container, false);

            forgotPassword_userInput = view.FindViewById<AutoCompleteTextView>(Resource.Id.ACTVforgotPWemail);
            sendRequest = view.FindViewById<Button>(Resource.Id.sendRequestButton);

            sendRequest.Click += SendRequest_Click;
            return view;
        }

        private void SendRequest_Click(object sender, EventArgs e)
        {
            OnCompletePasswordRequest.Invoke(this, new onCompletePasswordRequestArgs(forgotPassword_userInput.Text));
            this.Dismiss();
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
        }

    }
}