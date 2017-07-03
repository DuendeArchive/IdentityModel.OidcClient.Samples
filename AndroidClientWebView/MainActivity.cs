using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using IdentityModel.OidcClient;
using Java.Lang;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace AndroidClient
{
    [Activity(Label = "AndroidClient", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;
        private HttpClient _apiClient;
        private OidcClient _client;

        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);


            var authority = "https://demo.identityserver.io";

            var options = new OidcClientOptions(
                authority,
                "native.hybrid",
                "xoxo",
                "openid profile api",
                "io.identitymodel.native://callback",
                new AuthWebView(this));

            _client = new OidcClient(options);

            var result = await _client.LoginAsync();


            var sb = new StringBuilder(128);
            foreach (var claim in result.Claims)
            {
                sb.Append(string.Format("{0}: {1}\n", claim.Type, claim.Value));
            }

            sb.Append(string.Format("\n{0}: {1}\n", "refresh token", result.RefreshToken));
            sb.Append(string.Format("\n{0}: {1}\n", "access token", result.AccessToken));

            var editText1 = FindViewById<EditText>(Resource.Id.editText1);
            editText1.Text = sb.ToString();
            _apiClient = new HttpClient();
            _apiClient.SetBearerToken(result.AccessToken);
            _apiClient.BaseAddress = new Uri("https://api.identityserver.io/");


            Button button = FindViewById<Button>(Resource.Id.MyButton);

            button.Click += Button_Click;
        }

        private async void Button_Click(object sender, EventArgs e)
        {
            var editText1 = FindViewById<EditText>(Resource.Id.editText1);

            var result = await _apiClient.GetAsync("identity");
            if (!result.IsSuccessStatusCode)
            {
                editText1.Text = result.ReasonPhrase;
                return;
            }

            var content = await result.Content.ReadAsStringAsync();
            editText1.Text = JArray.Parse(content).ToString();
        }
    }
}

