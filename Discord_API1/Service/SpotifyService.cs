using System;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SpotifyAPI.Web;
using Swan;

namespace Discord_API1.Service
{
    public class SpotifyService
    {
        private static string Bot_id = "ef039d13351645e98dee0db229295352";  //Id of my spotify app
        private static string Bot_ids = "3c5a1f8875274be897f9fcc91cdcbfb7"; //secret Id 

        public static async Task<Tuple<int, string>> Search_song(string songName)
        {
            
            string query = HttpUtility.UrlEncode(songName, Encoding.Default);
            
            //Connection of Bot client
            var config = SpotifyClientConfig.CreateDefault();
            var request =
                new ClientCredentialsRequest(Bot_id, Bot_ids);
            var response = await new OAuthClient(config).RequestToken(request);
            var spotify = new SpotifyClient(config.WithToken(response.AccessToken));
            // --- 

            var result = await spotify.Search.Item(new SearchRequest(SearchRequest.Types.Track, query)); //Sending search request and creating json
            string data_json = result.Tracks.ToJson();
            
            dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(data_json);

            var link = data.Items[0].ExternalUrls.spotify; //АДРЕСА url ТРЕКУ
            var popularity = data.Items[0].Popularity; //популярність треку
            if (link == null || popularity == null)
            {
                throw new ArgumentException($"Song \"{songName}\" was not found.");
                
            }
            else
            {
                return Tuple.Create((int)popularity, link.ToString());
            }
        }
    }
}