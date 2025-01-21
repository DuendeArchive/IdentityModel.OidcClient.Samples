using IdentityModel.OidcClient;
using System;
using System.Windows;

namespace WpfWebView2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly OidcClientOptions _options = new OidcClientOptions()
        {
            Authority = "https://demo.duendesoftware.com/",
            ClientId = "interactive.public",
            Scope = "openid profile email",
            RedirectUri = "http://127.0.0.1/sample-wpf-app",
            Browser = new WpfEmbeddedBrowser(),
            Policy = new Policy
            {
                RequireIdentityTokenSignature = false
            }
        };
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var oidcClient = new OidcClient(_options);

            LoginResult loginResult;
            try
            {
                loginResult = await oidcClient.LoginAsync();
            }
            catch (Exception exception)
            {
                txbMessage.Text = $"Unexpected Error: {exception.Message}";
                return;
            }

            if (loginResult.IsError)
            {
                txbMessage.Text = loginResult.Error == "UserCancel" ? "The sign-in window was closed before authorization was completed." : loginResult.Error;
            }
            else
            {
                txbMessage.Text = loginResult.User.Identity.Name;
                btnLogout.Visibility = Visibility.Visible;
            }
        }

        private async void BtnLogout_OnClick(object sender, RoutedEventArgs e)
        {
            var oidcClient = new OidcClient(_options);

            LogoutResult logoutResult;
            try
            {
                logoutResult = await oidcClient.LogoutAsync();
            }
            catch (Exception exception)
            {
                txbMessage.Text = $"Unexpected Error: {exception.Message}";
                return;
            }

            if (logoutResult.IsError)
            {
                txbMessage.Text = logoutResult.Error;
            }
            else
            {
                txbMessage.Text = "Logged out.";
                btnLogout.Visibility = Visibility.Collapsed;
            }
        }
    }
}
