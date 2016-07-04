using System;
using Android.Content;
using Android.Gms.Common;
using Android.Util;
using Inbanker.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(VerifyPlayServices))]

namespace Inbanker.Droid
{

	public class VerifyPlayServices : ResultPlayService
	{

		public VerifyPlayServices()
		{

		}

		public bool IsPlayServicesAvailable()
		{


			int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(Forms.Context);
			if (resultCode != ConnectionResult.Success)
			{
				Log.Info("Retorno PlayService", "False");

				/*if (GoogleApiAvailability.Instance.IsUserResolvableError (resultCode))
					return GoogleApiAvailability.Instance.GetErrorString (resultCode);
				else
				{
					return "Sorry, this device is not supported";
					//Finish ();
				}*/
				return false;

			}
			else
			{
				Log.Info("Retorno PlayService", "True");

				var intent = new Intent(Forms.Context, typeof(RegistrationIntentService));
				Forms.Context.StartService(intent);

				return true;
			}
		}
	}

}

