using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Support.V4.App;
using Firebase.Messaging;

namespace MDispatch.Droid.StoreService
{
    [Service]
    //[IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        const string TAG = "FirebaseMsgService";

        public override void OnMessageReceived(RemoteMessage message)
        {
            var body = message.GetNotification();
            SendNotification(body.Body ,body.Title, body.ClickAction);
            Android.Util.Log.Debug(TAG, "From: " + message.From);
            Android.Util.Log.Debug(TAG, "Notification Message Body: " + message.GetNotification().Body);
        }

        void SendNotification(string messageBody, string title, string actionClick)
        {
            Intent intent = new Intent(this, typeof(MainActivity));
            const int pendingIntentId = 0;
            PendingIntent.GetActivity(this, pendingIntentId, intent, PendingIntentFlags.OneShot);
            PendingIntent pendingIntent = GetIntentOrder(actionClick);
            var notificationBuilder = new AndroidX.Core.App.NotificationCompat.Builder(this)
                                      .SetSmallIcon(Resource.Drawable.newOrder)
                                      .SetContentTitle(title)
                                      .SetContentText(messageBody)
                                      .SetAutoCancel(true)
                                      .SetContentIntent(pendingIntent)
                                      .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.All))
                                      .SetDefaults((int)NotificationDefaults.All)
                                      .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Ringtone))
                                      .SetPriority((int)NotificationPriority.High)
                                      .SetVisibility((int)NotificationVisibility.Public); 
            var notificationManager = NotificationManagerCompat.From(this);
            notificationManager.Notify(100, notificationBuilder.Build());
        }

        private PendingIntent GetIntentOrder(string actionClick)
        {
            PendingIntent pendingIntent = null;
            if (actionClick == "No Action")
            {
                pendingIntent = null;
            }
            else if (actionClick == "Oreder")
            {
                Intent intent = new Intent(this, typeof(MainActivity));
                const int pendingIntentId = 100;
                PendingIntent.GetActivity(this, pendingIntentId, intent, PendingIntentFlags.OneShot);
                pendingIntent = PendingIntent.GetActivity(this, 100, intent, PendingIntentFlags.OneShot);
            }
            else if (actionClick == "Driver")
            {
                Intent intent = new Intent(this, typeof(MainActivity));
                const int pendingIntentId = 50;
                PendingIntent.GetActivity(this, pendingIntentId, intent, PendingIntentFlags.OneShot);
                pendingIntent = PendingIntent.GetActivity(this, 50, intent, PendingIntentFlags.OneShot);
            }
            return pendingIntent;
        }
    }
}