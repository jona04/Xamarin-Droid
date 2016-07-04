using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Inbanker.Droid
{
	[Activity(Label = "InBanker", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);

			global::Xamarin.Forms.Forms.Init(this, bundle);


			string notification = this.Intent.GetStringExtra("notification");

			//LoadApplication(new App());

			////recebe parametros vindo da notificacao
			if (notification == null)
			{
				LoadApplication(new App("false",null));
			}
			else 
			{ 
				if (notification.Equals("receber_pedido"))
				{
					string transacao = this.Intent.GetStringExtra("transacao");
					var trans = JsonConvert.DeserializeObject<Transacao>(transacao);

					LoadApplication(new App(notification,trans));
					//App.Current.MainPage = new VerPedidoRecebido(trans);
				}
				else
				{
					string transacao = this.Intent.GetStringExtra("transacao");
					var trans = JsonConvert.DeserializeObject<Transacao>(transacao);

					LoadApplication(new App(notification, trans));
					//App.Current.MainPage = new VerPedidosEnviados(trans);

				}
			}
				
		}
	}
}

