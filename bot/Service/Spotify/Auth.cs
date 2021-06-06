using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;


namespace SpotifyBot.Service.Spotify
{
    public partial class SpotifyService
    {
        private EmbedIOAuthServer _server;
        public SpotifyClient _client;

        public async Task Auth()
        {
            // Make sure "http://localhost:5000/callback" is in your spotify application as redirect uri!
            _server = new EmbedIOAuthServer(new Uri("http://localhost:5000/callback"), 5000);
            await _server.Start();

            _server.ImplictGrantReceived += OnImplicitGrantReceived;
            _server.ErrorReceived += OnErrorReceived;

            var request = new LoginRequest(_server.BaseUri, "c86632e34b144f8ab51445565ed91573", LoginRequest.ResponseType.Token)
            {
                Scope = new List<string> { Scopes.UserReadEmail }
            };
            BrowserUtil.Open(request.ToUri());
        }

        private async Task OnImplicitGrantReceived(object sender, ImplictGrantResponse response)
        {
            await _server.Stop();
            _client = new SpotifyClient(response.AccessToken);
            
        }

        private async Task OnErrorReceived(object sender, string error, string state)
        {
            await _server.Stop();
            throw new AuthException($"Aborting authorization, error received: {error}");
            
        }
    }
}