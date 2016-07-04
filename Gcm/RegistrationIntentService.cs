using System;
using Android.App;
using Android.Content;
using Android.Gms.Gcm;
using Android.Gms.Gcm.Iid;
using Android.Preferences;
using Android.Util;
using Xamarin.Forms;

namespace Inbanker.Droid
{
	[Service(Exported = false)]
	class RegistrationIntentService : IntentService
	{

		ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Forms.Context);

		static object locker = new object();

		public RegistrationIntentService() : base("RegistrationIntentService") { }

		protected override void OnHandleIntent(Intent intent)
		{
			try
			{
				Log.Info("RegistrationIntentService", "Calling InstanceID.GetToken");
				lock (locker)
				{
					var instanceID = InstanceID.GetInstance(this);
					var token = instanceID.GetToken(
						"800377828099", GoogleCloudMessaging.InstanceIdScope, null);

					Log.Info("RegistrationIntentService", "GCM Registration Token: " + token);
					SendRegistrationToAppServer(token);
					Subscribe(token);
				}
			}
			catch (Exception e)
			{
				Log.Debug("RegistrationIntentService", "Failed to get a registration token");
				return;
			}
		}

		void SendRegistrationToAppServer(string token)
		{
			// Add custom implementation here as needed.

			ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Forms.Context);
			ISharedPreferencesEditor editor = prefs.Edit();
			editor.PutString("token_gcm", token);
			// editor.Commit();    // applies changes synchronously on older APIs
			editor.Apply();  // applies changes asynchronously on newer APIs

			string id = prefs.GetString("usu_id_face", "");
			string nome = prefs.GetString("usu_nome", "");
			string usu_img = prefs.GetString("usu_picture", "");

			ChamaWebService(id, nome, token, usu_img);

		}

		void Subscribe(string token)
		{
			var pubSub = GcmPubSub.GetInstance(this);
			pubSub.Subscribe(token, "/topics/global", null);
		}

		public async void ChamaWebService(string id, string nome, string token,string img)
		{
			ServiceWrapper serviceWrapper = new ServiceWrapper();

			//var result = await serviceWrapper.GetData("test");
			var result = await serviceWrapper.RegisterUserFormRequest(id, nome, token,img);
			Log.Debug("RegistrationIntentService", id + " - " + nome + " - " + token);
			Log.Debug("RegistrationIntentService", result);
		}

	}
}

