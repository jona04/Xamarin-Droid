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

[assembly: ExportRenderer(typeof(Login), typeof(Inbanker.Droid.LoginPageRenderer))]

namespace Inbanker.Droid
{
	public class LoginPageRenderer : PageRenderer
	{

		// On Android:
		IEnumerable<Account> accounts;
		Account facebook;

		Usuario eu;

		bool done = false;

		protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
		{
			base.OnElementChanged(e);

			if (!done)
			{

				// this is a ViewGroup - so should be able to load an AXML file and FindView<>
				var activity = this.Context as Activity;
				var auth = new OAuth2Authenticator(
					clientId: App.Current.Properties["clientId"].ToString(), // your OAuth2 client id
					scope: App.Current.Properties["scope"].ToString(), // the scopes for the particular API you're accessing, delimited by "+" symbols
					authorizeUrl: new Uri(App.Current.Properties["authorizeUrl"].ToString()),
					redirectUrl: new Uri(App.Current.Properties["redirectUrl"].ToString()));



				auth.Completed += async (sender, eventArgs) =>
				{
					if (eventArgs.IsAuthenticated)
					{

						// On Android:
						AccountStore.Create(Forms.Context).Save(eventArgs.Account, "Facebook");

						var parameters = new Dictionary<string, string>();
						//parameters.Add("fields", "friends{id,name,picture{url}},id,name,picture{url}");
						parameters.Add("fields", "id,name,picture.type(large),friends{id,name,picture.type(large){url}}");

						//var accessToken = eventArgs.Account.Properties ["access_token"].ToString ();
						//var expiresIn = Convert.ToDouble (eventArgs.Account.Properties ["expires_in"]);
						//var expiryDate = DateTime.Now + TimeSpan.FromSeconds (expiresIn);

						var request = new OAuth2Request("GET", new Uri("https://graph.facebook.com/me"), parameters, eventArgs.Account);
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

						////adicionamos os dados do usuario recem logado no objeto
						eu = new Usuario
						{
							id_usuario = id,
							nome_usuario = nome,
							url_img = picture_url,
						};

						var usu = JsonConvert.DeserializeObject<List<Amigos>>(lista_friends);

						ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Forms.Context);
						ISharedPreferencesEditor editor = prefs.Edit();
						//armazemos no shread preferences para capturar-mos em outras partes do projeto android
						editor.PutString("usu_id_face", id);
						editor.PutString("usu_nome", nome);
						editor.PutString("usu_picture", picture_url);
						// editor.Commit();    // applies changes synchronously on older APIs
						editor.Apply();  // applies changes asynchronously on newer APIs

						string token_gcm = prefs.GetString("token_gcm", "");

						//fazemos verificaçao do play service e registro do GCM
						VerifyPlayServices verify = new VerifyPlayServices();
						verify.IsPlayServicesAvailable();

						////friends.
						//await App.NavigateToLista(eu, usu);

						App.Current.MainPage = new MainPageCS(eu, usu);
						App.Current.Properties["access_token"] = eventArgs.Account.Properties["access_token"].ToString();
					}
					else
					{
						// Auth failed - The only way to get to this branch on Google is to hit the 'Cancel' button.
						App.Current.MainPage = new LoginPage();
						App.Current.Properties["access_token"] = "";
					}
				};

				//auth.AllowCancel = false;
				activity.StartActivity(auth.GetUI(activity));
				done = true;

			}


		}
	}
}


