using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
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

        public event EventHandler<OnMessageReceivedArgs> OnMessageReceivedEvent;

        public override void OnMessageReceived(RemoteMessage message)
        {
            base.OnMessageReceived(message);
            SendNotification(message.GetNotification().Body);

            LocalBroadcastManager broadcaster = LocalBroadcastManager.GetInstance(this);

            Intent intent = new Intent("message");
            intent.PutExtra("messageReceived", true);
            broadcaster.SendBroadcast(intent);

            OnMessageReceivedEvent.Invoke(this, new OnMessageReceivedArgs(true));
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
    
    class OnMessageReceivedArgs
    {
        public bool messageReceived { get; set; }

        public OnMessageReceivedArgs(bool received)
        {
            messageReceived = received;
        }
    }
   
}