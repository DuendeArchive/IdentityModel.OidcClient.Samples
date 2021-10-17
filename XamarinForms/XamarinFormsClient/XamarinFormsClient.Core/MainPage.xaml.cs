using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinFormsClient.Core
{
    [XamlCompilation(XamlCompilationOptions.Skip)]
    public partial class MainPage : ContentPage
    {
        OidcClient _client;
        LoginResult _result;

        Lazy<HttpClient> _apiClient = new Lazy<HttpClient>(() => new HttpClient());

        public MainPage()
        {
            InitializeComponent();

            Login.Clicked += Login_Clicked;
            CallApi.Clicked += CallApi_Clicked;

            var browser = DependencyService.Get<IBrowser>();

            var options = new OidcClientOptions
            {
                Authority = "https://demo.identityserver.io",
                ClientId = "interactive.public",
                Scope = "openid profile email api offline_access",
                RedirectUri = "xamarinformsclients://callback",
                Browser = browser
            };

            _client = new OidcClient(options);
            _apiClient.Value.BaseAddress = new Uri("https://demo.identityserver.io/");
        }

        private async void Login_Clicked(object sender, EventArgs e)
        {
            _result = await _client.LoginAsync(new LoginRequest());

            if (_result.IsError)
            {
                OutputText.Text = _result.Error;
                return;
            }

            var sb = new StringBuilder(128);
            foreach (var claim in _result.User.Claims)
            {
                sb.AppendFormat("{0}: {1}\n", claim.Type, claim.Value);
            }

            sb.AppendFormat("\n{0}: {1}\n", "refresh token", _result?.RefreshToken ?? "none");
            sb.AppendFormat("\n{0}: {1}\n", "access token", _result.AccessToken);

            OutputText.Text = sb.ToString();

            _apiClient.Value.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _result?.AccessToken ?? "");
        }

        private async void CallApi_Clicked(object sender, EventArgs e)
        {

            var result = await _apiClient.Value.GetAsync("api/test");

            if (result.IsSuccessStatusCode)
            {
                OutputText.Text = JsonDocument.Parse(await result.Content.ReadAsStringAsync()).RootElement.GetRawText();
            }
            else
            {
                OutputText.Text = result.ReasonPhrase;
            }
        }
    }
}