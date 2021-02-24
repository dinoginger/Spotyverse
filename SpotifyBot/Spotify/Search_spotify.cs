using System.Threading.Tasks;
using Discord;
using SpotifyAPI.Web;
using Swan;

namespace SpotifyBot.Spotify
{
    public partial class SpotifyService
    {
        public async Task<Embed> Search(string songName)
        {
            //Getting tokens from our json.
            GetSpotifyTokens();

            //Connection of Bot client
            var config = SpotifyClientConfig.CreateDefault();
            var request =
                new ClientCredentialsRequest(Bot_id, Bot_ids);
            var response = await new OAuthClient(config).RequestToken(request);
            var spotify = new SpotifyClient(config.WithToken(response.AccessToken));
            // --- 
            try
            {
                var result =
                    await spotify.Search.Item(new SearchRequest(SearchRequest.Types.All,
                        songName)); //Sending search request and creating json

                string data_json = result.Tracks.ToJson();

                dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(data_json);
            }
            catch
            {
                
            }

            return null;


        }
    }
}