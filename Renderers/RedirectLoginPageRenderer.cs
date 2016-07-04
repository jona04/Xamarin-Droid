using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Inbanker;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Auth;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Android.Preferences;
using Android.App;
using Android.Util;

[assembly: ExportRenderer(typeof(RedirectLogin), typeof(Inbanker.Droid.RedirectLoginPageRenderer))]

namespace Inbanker.Droid
{
	public class RedirectLoginPageRenderer : PageRenderer
	{

		bool done = false;

		// On Android:
		IEnumerable<Account> accounts;
		Account facebook;

		ISharedPreferences prefs;
		ISharedPreferencesEditor editor;

		Usuario eu;

		protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
		{
			base.OnElementChanged(e);

			// On Android:
			accounts = AccountStore.Create(Forms.Context).FindAccountsForService("Facebook");

			//se existir uma conta armazenada fazemos o login, se nao somos redirecionados para a tela de login
			if (accounts.Count() > 0)
			{
				Log.Debug("Account Facebook", "maior que zero");
				var enumerable = accounts as IList<Account> ?? accounts.ToList();
				facebook = enumerable.First();

				VerificaUser();

			}
			else
			{
				Log.Debug("Account Facebook", "menor que zero");
			}

		}

		public async void VerificaUser()
		{
			var parameters = new Dictionary<string, string>();
			//parameters.Add("fields", "friends{id,name,picture{url}},id,name,picture.type(large)");
			parameters.Add("fields", "id,name,picture.type(large),friends{id,name,picture.type(large){url}}");

			var request = new OAuth2Request("GET", new Uri("https://graph.facebook.com/me"), parameters, facebook);
			var response = await request.GetResponseAsync();
			var obj = JObject.Parse(response.GetResponseText());

			var picture = obj["picture"].ToString();
			var friends = obj["friends"].ToString();
			var id = obj["id"].ToString().Replace("\"", "");
			var nome = obj["name"].ToString().Replace("\"", "");

			var obj_friends = JObject.Parse(friends);
			var lista_friends = obj_friends["data"].ToString();

			var obj_picture = JObject.Parse(picture);
			var picture_data = obj_picture["data"].ToString();

			var obj_picture2 = JObject.Parse(picture_data);
			var picture_url = obj_picture2["url"].ToString();

			//adicionamos os dados do usuario recem logado no objeto
			eu = new Usuario
			{
				id_usuario = id,
				nome_usuario = nome,
				url_img = picture_url,
			};

			var usu = JsonConvert.DeserializeObject<List<Amigos>>(lista_friends);

			//armazenamos dados no sharedpreferences
			//prefs = PreferenceManager.GetDefaultSharedPreferences(Forms.Context);
			//editor = prefs.Edit();
			//editor.PutString("usu_id_face", id);
			//editor.PutString("usu_nome", nome);
			//// editor.Commit();    // applies changes synchronously on older APIs
			//editor.Apply();  // applies changes asynchronously on newer APIs

			//fazemos verificaçao do play service e registro do GCM
			//VerifyPlayServices verify = new VerifyPlayServices();
			//verify.IsPlayServicesAvailable ();


			App.Current.MainPage = new MainPageCS(eu, usu);
			App.Current.Properties["access_token"] = facebook.Properties["access_token"];


		}
	}
}


