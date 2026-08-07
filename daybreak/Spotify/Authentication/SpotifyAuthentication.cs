using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using System.IO;
using System.Threading.Tasks;
using static SpotifyAPI.Web.Scopes;

namespace daybreak.Spotify.Authentication
{
    public class SpotifyAuthentication
    {
        private readonly string credentialsPath;
        private readonly string clientId;
        private readonly EmbedIOAuthServer server;

        public SpotifyClient? Client { get; private set; }

        public SpotifyAuthentication()
        {
            clientId = Configuration.AppConfiguration.Configuration["Spotify:ClientId"]
                ?? throw new Exception("Spotify Client ID not found.");

            credentialsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Daybreak",
                "spotify.json");

            Directory.CreateDirectory(Path.GetDirectoryName(credentialsPath)!);

            server = new EmbedIOAuthServer(
                new Uri("http://127.0.0.1:5543/callback"),
                5543);
        }

        public async Task AuthenticateAsync()
        {
            if (File.Exists(credentialsPath))
            {
                await LoginFromSavedToken();
            }
            else
            {
                await FirstTimeLogin();
            }
        }

        private async Task LoginFromSavedToken()
        {
            var json = await File.ReadAllTextAsync(credentialsPath);

            var token =
                Newtonsoft.Json.JsonConvert.DeserializeObject<PKCETokenResponse>(json);

            System.Diagnostics.Debug.WriteLine(
                $"Loaded refresh token: {token.RefreshToken}");

            if (token == null)
                throw new Exception("Unable to read Spotify credentials.");

            var savedRefreshToken = token.RefreshToken;

            var authenticator = new PKCEAuthenticator(clientId, token);

            System.Diagnostics.Debug.WriteLine("PKCEAuthenticator created.");

            authenticator.TokenRefreshed += (_, newToken) =>
            {
                System.Diagnostics.Debug.WriteLine("TokenRefreshed event fired.");

                System.Diagnostics.Debug.WriteLine(
                    $"Original token refresh: '{token.RefreshToken}'");

                System.Diagnostics.Debug.WriteLine(
                    $"New token refresh: '{newToken.RefreshToken}'");

                var updatedToken = new PKCETokenResponse
                {
                    AccessToken = newToken.AccessToken,
                    TokenType = newToken.TokenType,
                    ExpiresIn = newToken.ExpiresIn,
                    Scope = newToken.Scope,
                    RefreshToken = savedRefreshToken,
                    CreatedAt = newToken.CreatedAt,
                };

                File.WriteAllText(
                    credentialsPath,
                    Newtonsoft.Json.JsonConvert.SerializeObject(updatedToken));
            };

            var config = SpotifyClientConfig
                .CreateDefault()
                .WithAuthenticator(authenticator);

            Client = new SpotifyClient(config);

            System.Diagnostics.Debug.WriteLine("SpotifyClient created.");
        }

        private async Task FirstTimeLogin()
        {
            var (verifier, challenge) = PKCEUtil.GenerateCodes();

            server.AuthorizationCodeReceived += async (sender, response) =>
            {
                await server.Stop();

                var token = await new OAuthClient().RequestToken(
                    new PKCETokenRequest(
                        clientId,
                        response.Code,
                        server.BaseUri,
                        verifier));

                await File.WriteAllTextAsync(
                    credentialsPath,
                    Newtonsoft.Json.JsonConvert.SerializeObject(token));

                await LoginFromSavedToken();

                var me = await Client!.UserProfile.Current();

                System.Diagnostics.Debug.WriteLine(
                    $"Connected to Spotify as: {me.DisplayName}");
            };

            await server.Start();

            var request = new LoginRequest(
                server.BaseUri,
                clientId,
                LoginRequest.ResponseType.Code)
            {
                CodeChallenge = challenge,
                CodeChallengeMethod = "S256",

                Scope = new List<string>
        {
            UserReadEmail,
            UserReadPrivate,

            UserReadPlaybackState,
            UserModifyPlaybackState,
            UserReadCurrentlyPlaying,

            PlaylistReadPrivate,
            PlaylistReadCollaborative
        }
            };

            BrowserUtil.Open(request.ToUri());
        }
    }
}