using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SpotifyAPI.Web;
using Swan;

namespace SpotifyBot.Spotify
{
    
    public partial class SpotifyService
    {
        private static string Bot_id;  //Id of my spotify app
        private static string Bot_ids; //secret Id 

        private static readonly string configPath = @"C:\Users\Марко\OneDrive\Desktop\Discord\TestBotStuff\TestBot_\bot\_config.json";

        private static void GetSpotifyTokens() //Method to get spotify tokens
        {
            StreamReader r = new StreamReader(configPath);
            string json = r.ReadToEnd();
            dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            Bot_id = data.Spotify_id;
            Bot_ids = data.Spotify_ids;

        }
/// <summary>
/// This method is created for command !listen.
/// Returns string with genres of songs artist and popularity score.
/// </summary>
/// <param name="songName"></param>
/// <returns></returns>
/// <exception cref="ArgumentException"></exception>
        public static async Task<Tuple<int, string>> Listen(string songName)
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
                var result = await spotify.Search.Item(new SearchRequest(SearchRequest.Types.All, songName)); //Sending search request and creating json
            
            string data_json = result.Tracks.ToJson();
            
            dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(data_json);
            
            var popularity = data.Items[0].Popularity; //популярність треку
            
            //GenreSearch
            string artistid = data.Items[0].Album.Artists[0].Id.ToString();
            var artist = await spotify.Artists.Get(artistid); 
            var artistname = data.Items[0].Artists[0].Name.ToString(); 
            string genres_string = "";
            foreach (var genre in artist.Genres.ToArray())
            {
                genres_string = genres_string + "+" + genre;
            }
            
            return Tuple.Create((int)popularity,genres_string);
            
            }
            catch (Exception e)
            {
                throw new ArgumentException($"Song \"{songName}\" was not found.");
                
            }

        }

/// <summary>
/// This method created for listen command.
/// </summary>
/// <param name="all_genres_string"></param>
/// <returns></returns>
        public static string GetTopGenres(string all_genres_string)
        {
            if (all_genres_string == null)
            {
                throw new ArgumentNullException("genre_string is null. At GetTopGenres method.");
            }
            
            string topgenres = "";
            var genre_list = all_genres_string
                .Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries)
                .GroupBy(w => w)
                .OrderByDescending(g => g.Count());

            foreach (var word in genre_list){
                Console.WriteLine("{0}x {1}", word.Count(), word.Key);
            }

            int mm = 0;
            foreach (var genre in genre_list )
            {
                if (genre.Count() >= 2 && mm < 3)
                {
                    topgenres = topgenres + "/" + genre.Key;
                    mm++;
                }
            }

            return topgenres;
            //TODO: return genre string look better, do stuff with slashes
            
        }
    }
}