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
using carServiceApp.Activities;
using Firebase.Auth;
using Firebase.Iid;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;

namespace carServiceApp.My_Classes.myFirebaseMessaging
{
    [Service]
    [IntentFilter (new [] {"com.google.firebase.INSTANCE_ID_EVENT"})]

    class MyFirebaseIdService:FirebaseInstanceIdService
    {
        private string id;
        public override void OnTokenRefresh()
        {
            base.OnTokenRefresh();
            Android.Util.Log.Debug("Refreshed Token:", FirebaseInstanceId.Instance.Token);

            FirebaseUser users = FirebaseAuth.GetInstance(loginActivity.app).CurrentUser;
            if (users != null)
            {
                id = users.Uid;
                var firebase = new FirebaseClient(loginActivity.FirebaseURL);
                var item = firebase.Child("tokens").Child(id).PutAsync<string>(FirebaseInstanceId.Instance.Token);
            }
            else
            {
                var instanceId = FirebaseInstanceId.Instance;
                instanceId.DeleteInstanceId();
            }
        }
       
    }
}