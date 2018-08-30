using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using Firebase.Messaging;

namespace carServiceApp.My_Classes.myFirebaseMessaging
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]

    class MyFirebaseMessagingService : FirebaseMessagingService
    {

        public override void OnMessageReceived(RemoteMessage message)
        {
            base.OnMessageReceived(message);
            SendNotification(message.GetNotification().Body);

            LocalBroadcastManager broadcaster = LocalBroadcastManager.GetInstance(this);

            Intent intent = new Intent("message");
            intent.PutExtra("messageReceived", true);
            broadcaster.SendBroadcast(intent);

            Context myContext = Android.App.Application.Context;
            appPreferences app = new appPreferences(myContext);

            string newMessage = message.GetNotification().Body;
            string data = message.GetNotification().Title;
            
            app.saveAccesKey(data);
        }

        private void SendNotification(string body)
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

            var defaultSoundUri = RingtoneManager.GetDefaultUri(RingtoneType.Notification);
            var notificationBuilder = new NotificationCompat.Builder(this)
                .SetContentTitle("Moj servis obavijest")
                .SetContentText(body)
                .SetSmallIcon(Resource.Drawable.ifsedan285810)
                .SetAutoCancel(true)
                .SetSound(defaultSoundUri)
                .SetContentIntent(pendingIntent);

            var notificationManager = NotificationManager.FromContext(this);
            notificationManager.Notify(0, notificationBuilder.Build());
        }
    }

    public class appPreferences
    {
        private ISharedPreferences mySharedPreferences;
        private ISharedPreferencesEditor mySharedPreferencesEditor;
        private Context mContext;

        private static string PREFERENCE_ACCESS_KEY = "PREFERENCE_ACCESS_KEY";

        public appPreferences(Context context)
        {
            this.mContext = context;
            mySharedPreferences = PreferenceManager.GetDefaultSharedPreferences(mContext);
            mySharedPreferencesEditor = mySharedPreferences.Edit();
        }

        public void saveAccesKey(string key)
        {
            mySharedPreferencesEditor.PutString(PREFERENCE_ACCESS_KEY, key);
            mySharedPreferencesEditor.Commit();
        }

        public string getAccesKey()
        {
            return mySharedPreferences.GetString(PREFERENCE_ACCESS_KEY, "");
        }
    }
   
}