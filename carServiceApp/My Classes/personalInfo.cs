using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using carServiceApp.Activities;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;

namespace carServiceApp.My_Classes
{
    class personalInfo:DialogFragment
    {
        private View view;
        public  EditText userInput_ime;
        private EditText userInput_prezime;
        private EditText userInput_broj;
        private EditText userInput_email;
        private EditText userInpu_ulicaibr;
        private EditText userInput_mjesto;
        private Button   save;

        private string id;
        private string key;
        connection con = new connection();

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            
            view = inflater.Inflate(Resource.Layout.personalInfo, container, false);

            userInput_ime     = view.FindViewById<EditText>(Resource.Id.PIinputIme);
            userInput_prezime = view.FindViewById<EditText>(Resource.Id.PIinputPrezime);
            userInput_email   = view.FindViewById<EditText>(Resource.Id.PIinputEmail);
            userInput_broj    = view.FindViewById<EditText>(Resource.Id.PIinputBrojTelefona);
            userInput_mjesto  = view.FindViewById<EditText>(Resource.Id.PIinputMjesto);
            userInpu_ulicaibr = view.FindViewById<EditText>(Resource.Id.PIinputUlicaIBroj);
            save = view.FindViewById<Button>(Resource.Id.savePersonalInfo);

            var auth = FirebaseAuth.GetInstance(loginActivity.app).CurrentUser;
            id = auth.Uid;
           
            getUserData();

            save.Click += Save_ClickAsync;
            return view;
        }

        private async void Save_ClickAsync(object sender, EventArgs e)
        {
            var firebase = new FirebaseClient(loginActivity.FirebaseURL);

            User user = new User();
            user.name     = userInput_ime.Text;
            user.lastName = userInput_prezime.Text;
            user.email    = userInput_email.Text;
            user.phone    = userInput_broj.Text;
            user.adress   = userInpu_ulicaibr.Text;
            user.city     = userInput_mjesto.Text;
            user.uid      = id;

            con.db.Execute("UPDATE User SET name = '" + user.name + "', lastName = '" + user.lastName + "', phone = '" + user.phone + "', " +
               "email = '" + user.email + "', city = '" + user.city + "', adress = '" + user.adress + "' WHERE uid = '" + id + "' ");

            var getKey = await firebase.Child("users").Child(id).OnceAsync<Account>();
            foreach (var item in getKey)
            {
                key = item.Key;
            }

            try
            {
                var data = firebase.Child("users").Child(id).Child(key).PutAsync<User>(user);
            }
            catch (Exception)
            {
                Toast.MakeText(view.Context, "Neuspješno", ToastLength.Long).Show();
            }
            Toast.MakeText(view.Context, "Uspješno spremljeno", ToastLength.Long).Show();
            Thread.Sleep(2000);
            this.Dismiss();
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
        }

        public void getUserData ()
        {
            List<User> data = con.db.Query<User>("SELECT * FROM User WHERE uid = '" + id + "'");
            foreach (var item in data)
            {
                    userInput_ime.Text     = item.name;
                    userInput_prezime.Text = item.lastName;
                    userInput_broj.Text    = item.phone;
                    userInput_email.Text   = item.email;
                    userInput_mjesto.Text  = item.city;
                    userInpu_ulicaibr.Text = item.adress;
            }

            //    var firebase = new FirebaseClient(loginActivity.FirebaseURL);
            //var data = await firebase.Child("users").Child(id).OnceAsync<User>();

            //foreach (var item in data)
            //{
            //   
            //}
        }

    }
}