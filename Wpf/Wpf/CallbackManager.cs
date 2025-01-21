using System;
using System.IO;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Wpf;

/// <summary>
///  The CallbackManager coordinates communication between two instances of the
///  application that are needed because we are using a custom url scheme.
///
///  First, the main instance of the application initiates the OIDC flow by
///  opening a browser window and navigating to the authorize endpoint, passing
///  the OIDC parameters. Notably the callback_uri passed here uses a custom
///  scheme, like this: oidcclient-wpf-sample://callback.
///
///  When the user finishes authentication in the browser, the response from the
///  identity provider is a redirect back to the callback uri at the custom
///  scheme. This redirect includes query parameters needed to continue the OIDC
///  flow, such as the authorization code. The OidcClient library needs those
///  parameters to continue processing. 
///
/// The custom scheme has been configured in the registry to use this
/// application (see see RegistryConfig.cs). When the browser is redirected to
/// the custom scheme it will therefore invoke this application. But this means
/// that a second instance of the application is actually started, and we
/// therefore have the OIDC state that initiated the challenge in one process,
/// while we have the callback url needed to continue the flow in another
/// process. 
///
/// The callback url is sent from one process to the other using a named pipe.
/// This class manages those named pipes and provides an abstraction around the
/// communication process.
/// </summary>
class CallbackManager
{
    private readonly string _name;

    public CallbackManager(string name)
    {
        _name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public int ClientConnectTimeoutSeconds { get; set; } = 1;

    // This method is invoked when the callback opens the application. It sends
    // the callback url back to the main instance of the application.
    public async Task RunClient(string args)
    {
        using (var client = new NamedPipeClientStream(".", _name, PipeDirection.Out))
        {
            await client.ConnectAsync(ClientConnectTimeoutSeconds * 1000);

            using (var sw = new StreamWriter(client) { AutoFlush = true })
            {
                await sw.WriteAsync(args);
            }
        }
    }

    // This method is invoked from the main instance of the application. It
    // receives the callback url from the secondary instance of the application
    // started by the callback redirect.
    public async Task<string> RunServer()
    {
        await using var server = new NamedPipeServerStream(_name, PipeDirection.In);
        await server.WaitForConnectionAsync();

        using var sr = new StreamReader(server);
        var msg = await sr.ReadToEndAsync();
        return msg;
    }
}
