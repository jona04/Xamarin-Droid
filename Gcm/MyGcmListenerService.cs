using System;
using Android.App;
using Android.Content;
using Android.Gms.Gcm;
using Android.Media;
using Android.OS;
using Android.Preferences;
using Android.Util;
using Xamarin.Forms;

namespace Inbanker.Droid
{
	[Service(Exported = false), IntentFilter(new[] { "com.google.android.c2dm.intent.RECEIVE" })]
	public class MyGcmListenerService : GcmListenerService
	{

		ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Forms.Context);
		string token_gcm;

		public override void OnMessageReceived(string from, Bundle data)
		{
			var registration_ids = data.GetString("registration_ids");
			var message = data.GetString("message");
			var tipo = data.GetString("tipo");

			if (tipo.Equals("resposta_pedido"))
			{

				//var trans_valor = data.GetString("trans_valor");
				//var trans_vencimento = data.GetString("trans_vencimento");
				//var trans_data = data.GetString("trans_data");
				//var nome_user = data.GetString("nome_user");
				//var trans_id_user2 = data.GetString("trans_id_user2");
				//var trans_resposta_pedido = data.GetString("trans_resposta_pedido");
				//var trans_dias = data.GetString("trans_dias");

				var transacao = data.GetString("transacao");

				Log.Debug("MyGcmListenerService", "From:    " + from);
				Log.Debug("MyGcmListenerService", "registration_ids: " + registration_ids);
				Log.Debug("MyGcmListenerService", "Message: " + message);


				token_gcm = prefs.GetString("token_gcm", "");
				if (token_gcm.Equals(registration_ids))
					SendNotificationRespostaPedido(message,transacao);

			}
			else if (tipo.Equals("envio_pedido"))
			{

				var transacao = data.GetString("transacao");

				Log.Debug("MyGcmListenerService", "From:    " + from);
				Log.Debug("MyGcmListenerService", "registration_ids: " + registration_ids);
				Log.Debug("MyGcmListenerService", "Message: " + message);

				token_gcm = prefs.GetString("token_gcm", "");
				if (token_gcm.Equals(registration_ids))
					SendNotificationEnvioPedido(message, transacao);

			}



		}

		void SendNotificationRespostaPedido(string message, string trans)
		{

			Log.Debug("Notification", "Chegou aqui notificacao");

			var intent = new Intent(this, typeof(MainActivity));
			//Bundle b = new Bundle();
			//b.PutBoolean("notification",true);
			//b.putInt("num_gcm", id);
			intent.PutExtra("notification", "resposta_pedido");

			intent.PutExtra("transacao", trans);

			intent.AddFlags(ActivityFlags.ClearTop);
			var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.UpdateCurrent);

			var notificationBuilder = new Notification.Builder(this)
				.SetSmallIcon(Resource.Drawable.icon)
				.SetContentTitle("InBanker")
				.SetContentText(message)
				.SetAutoCancel(true)
				.SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))
				.SetContentIntent(pendingIntent);


			Notification.BigTextStyle bigText = new Notification.BigTextStyle();
			bigText.BigText(message);
			notificationBuilder.SetStyle(bigText);

			var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
			notificationManager.Notify(0, notificationBuilder.Build());
		}

		void SendNotificationEnvioPedido(string message, string trans)
		{

			Log.Debug("Notification", "Chegou aqui notificacao");

			var intent = new Intent(this, typeof(MainActivity));
			intent.PutExtra("notification", "receber_pedido");
			intent.PutExtra("transacao", trans);

			intent.AddFlags(ActivityFlags.ClearTop);
			var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.UpdateCurrent);

			var notificationBuilder = new Notification.Builder(this)
				.SetSmallIcon(Resource.Drawable.icon)
				.SetContentTitle("InBanker")
				.SetContentText(message)
				.SetAutoCancel(true)
				.SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))
				.SetContentIntent(pendingIntent);


			Notification.BigTextStyle bigText = new Notification.BigTextStyle();
			bigText.BigText(message);
			notificationBuilder.SetStyle(bigText);

			var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
			notificationManager.Notify(0, notificationBuilder.Build());
		}
	}
}


