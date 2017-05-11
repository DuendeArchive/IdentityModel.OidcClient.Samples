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
        private HttpClient _apiClient;
        private OidcClient _client;

        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);


            Button callApibut = FindViewById<Button>(Resource.Id.btn_call_api);
            callApibut.Click += callApi_Click;


            Button login_btn = FindViewById<Button>(Resource.Id.btn_login);
            login_btn.Click += Login_Click;

        }


        private async void Login_Click(object sender, EventArgs e)
        {
            var txtResult = FindViewById<EditText>(Resource.Id.txtResult);
            txtResult.Text = "Opening Browser .....";

            var authority = "https://demo.identityserver.io";

            var options = new OidcClientOptions(
                 authority: authority,
                 clientId: "native",
                 clientSecret: "secret",
                 scope: "openid profile api offline_access",
                 redirectUri: "io.identitymodel.native://callback",
                 webView: new AuthWebView(this));

            _client = new OidcClient(options);

            var result = await _client.LoginAsync();


            if (!string.IsNullOrEmpty(result.Error))
            {
                txtResult.Text = result.Error;
                return;
            }



            var sb = new StringBuilder();
            foreach (var claim in result.Claims)
            {
                sb.Append($"{claim.Type}: {claim.Value}\n");
            }

            sb.Append($"\n refresh token: {result.RefreshToken}\n");
            sb.Append($"\n access token: {result.AccessToken}\n");


            txtResult.Text = sb.ToString();

            _apiClient = new HttpClient(result.Handler);
            _apiClient.BaseAddress = new Uri("https://demo.identityserver.io/api/");

        }

        private async void callApi_Click(object sender, EventArgs e)
        {
            var txtResult = FindViewById<EditText>(Resource.Id.txtResult);
            txtResult.Text = "Calling API.....";

            var result = await _apiClient.GetAsync("test");
            if (!result.IsSuccessStatusCode)
            {
                txtResult.Text = result.ReasonPhrase;
                return;
            }

            var content = await result.Content.ReadAsStringAsync();
            txtResult.Text = JArray.Parse(content).ToString();
        }
    }
}

