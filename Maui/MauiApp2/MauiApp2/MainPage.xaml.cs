using System.Text;
using IdentityModel.Client;
using IdentityModel.OidcClient;

namespace MauiApp2;

public partial class MainPage
{
    private readonly OidcClient _client;

    public MainPage(OidcClient client)
    {
        InitializeComponent();
        _client = client;
    }

    private async void OnCounterClicked(object sender, EventArgs e)
    {
        var result = await _client.LoginAsync();

        if (result.IsError)
        {
            editor.Text = result.Error;
            return;
        }

        var sb = new StringBuilder(128);

        sb.AppendLine("claims:");
        foreach (var claim in result.User.Claims)
        {
            sb.AppendLine($"{claim.Type}: {claim.Value}");
        }

        sb.AppendLine();
        sb.AppendLine("access token:");
        sb.AppendLine(result.AccessToken);

        if (!string.IsNullOrWhiteSpace(result.RefreshToken))
        {
            sb.AppendLine();
            sb.AppendLine("access token:");
            sb.AppendLine(result.AccessToken);
        }

        editor.Text = sb.ToString();
    }
}