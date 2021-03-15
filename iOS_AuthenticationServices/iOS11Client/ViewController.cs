using System;
using System.Net.Http;
using System.Text;
using IdentityModel.Client;
using IdentityModel.OidcClient;
using Microsoft.IdentityModel.Logging;
using UIKit;

namespace iOS11Client
{
    public partial class ViewController : UIViewController
    {
        OidcClient _client;
        User _user;

        protected ViewController(IntPtr handle) : base(handle)
        {
            IdentityModelEventSource.ShowPII = true;

            var options = new OidcClientOptions
            {
                Authority = "https://demo.identityserver.io",
                ClientId = "interactive.public",
                Scope = "openid profile email api offline_access",

                RedirectUri = "SFAuthenticationSessionExample://callback",
                PostLogoutRedirectUri = "SFAuthenticationSessionExample://callback",

                IdentityTokenValidator = new JwtHandlerIdentityTokenValidator(),
                Browser = new ASWebAuthenticationSessionBrowser()
            };

            _client = new OidcClient(options);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            LoginButton.TouchUpInside += LoginButton_TouchUpInside;
            LogoutButton.TouchUpInside += LogoutButton_TouchUpInside;

            RefreshButton.TouchUpInside += RefreshButton_TouchUpInside;
            CallApiButton.TouchUpInside += CallApiButton_TouchUpInside;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        private async void LoginButton_TouchUpInside(object sender, EventArgs e)
        {
            var result = await _client.LoginAsync(new LoginRequest());

            if (result.IsError)
            {
                OutputText.Text = result.Error;
                return;
            }

            SetUser(result);
            ShowUser();
        }

        // logout is currently not support by SFAuthenticationSession
        // https://forums.developer.apple.com/thread/91647
        private async void LogoutButton_TouchUpInside(object sender, EventArgs e)
        {
            OutputText.Text = "";

            var request = new LogoutRequest
            {
                IdTokenHint = _user.IdentityToken
            };

            _user = null;
            await _client.LogoutAsync(request);
        }

        private async void RefreshButton_TouchUpInside(object sender, EventArgs e)
        {
            if (_user?.RefreshToken != null)
            {
                var result = await _client.RefreshTokenAsync(_user.RefreshToken);

                if (result.IsError)
                {
                    OutputText.Text = result.Error;
                    return;
                }

                _user.RefreshToken = result.RefreshToken;
                _user.AccessToken = result.AccessToken;

                ShowUser();
            }
        }

        private async void CallApiButton_TouchUpInside(object sender, EventArgs e)
        {
            if (_user?.AccessToken != null)
            {
                var client = new HttpClient();
                client.SetBearerToken(_user.AccessToken);

                var response = await client.GetAsync("https://demo.identityserver.io/api/test");
                if (!response.IsSuccessStatusCode)
                {
                    OutputText.Text = response.ReasonPhrase;
                    return;
                }

                var content = await response.Content.ReadAsStringAsync();
                //OutputText.Text = JArray.Parse(content).ToString();
            }
        }

        private void ShowUser()
        {
            if (_user == null) return;

            var sb = new StringBuilder(128);
            foreach (var claim in _user.Claims.Claims)
            {
                sb.AppendFormat("{0}: {1}\n", claim.Type, claim.Value);
            }

            sb.AppendFormat("\n{0}: {1}\n", "refresh token", _user?.RefreshToken ?? "none");
            sb.AppendFormat("\n{0}: {1}\n", "access token", _user.AccessToken);

            OutputText.Text = sb.ToString();
        }

        private void SetUser(LoginResult result)
        {
            var user = new User
            {
                IdentityToken = result.IdentityToken,
                AccessToken = result.AccessToken,
                RefreshToken = result.RefreshToken,

                Claims = result.User,
                Handler = result.RefreshTokenHandler
            };

            _user = user;
        }
    }
}