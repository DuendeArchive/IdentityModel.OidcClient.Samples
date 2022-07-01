using IdentityModel.OidcClient;
using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace ConsoleSystemBrowser
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("+-----------------------+");
            Console.WriteLine("|  Sign in with OIDC    |");
            Console.WriteLine("+-----------------------+");
            Console.WriteLine("");
            Console.WriteLine("Press any key to sign in...");
            Console.ReadKey();

            Program p = new Program();
            p.SignIn();

            Console.ReadKey();
        }

        private async void SignIn()
        {
            // create a redirect URI using an available port on the loopback address.
            string redirectUri = string.Format("http://127.0.0.1:7890/");
            Console.WriteLine("redirect URI: " + redirectUri);

            // create an HttpListener to listen for requests on that redirect URI.
            var http = new HttpListener();
            http.Prefixes.Add(redirectUri);
            Console.WriteLine("Listening..");
            http.Start();

            var options = new OidcClientOptions
            {
                Authority = "https://demo.duendesoftware.com",
                ClientId = "interactive.public",
                Scope = "openid profile api",
                RedirectUri = redirectUri,
                Flow = OidcClientOptions.AuthenticationFlow.AuthorizationCode
            };
                
            var client = new OidcClient(options);
            var state = await client.PrepareLoginAsync();

            Console.WriteLine($"Start URL: {state.StartUrl}");
            
            // open system browser to start authentication
            Process.Start(state.StartUrl);

            // wait for the authorization response.
            var context = await http.GetContextAsync();

            var formData = GetRequestPostData(context.Request);

            // Brings the Console to Focus.
            BringConsoleToFront();

            // sends an HTTP response to the browser.
            var response = context.Response;
            string responseString = string.Format("<html><head><meta http-equiv='refresh' content='10;url=https://demo.duendesoftware.com'></head><body>Please return to the app.</body></html>");
            var buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            var responseOutput = response.OutputStream;
            await responseOutput.WriteAsync(buffer, 0, buffer.Length);
            responseOutput.Close();

            Console.WriteLine($"Form Data: {formData}");
            var result = await client.ProcessResponseAsync(formData, state);

            if (result.IsError)
            {
                Console.WriteLine("\n\nError:\n{0}", result.Error);
            }
            else
            {
                Console.WriteLine("\n\nClaims:");
                foreach (var claim in result.User.Claims)
                {
                    Console.WriteLine("{0}: {1}", claim.Type, claim.Value);
                }

                Console.WriteLine();
                Console.WriteLine("Access token:\n{0}", result.AccessToken);

                if (!string.IsNullOrWhiteSpace(result.RefreshToken))
                {
                    Console.WriteLine("Refresh token:\n{0}", result.RefreshToken);
                }
            }

            http.Stop();
        }

        // Hack to bring the Console window to front.
        // ref: http://stackoverflow.com/a/12066376
        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public void BringConsoleToFront()
        {
            SetForegroundWindow(GetConsoleWindow());
        }

        public static string GetRequestPostData(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
            {
                return null;
            }

            using (var body = request.InputStream)
            {
                using (var reader = new System.IO.StreamReader(body, request.ContentEncoding))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
