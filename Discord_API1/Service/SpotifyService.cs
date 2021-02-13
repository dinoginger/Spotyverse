using System;
using System.Linq;
using System.Threading.Tasks;
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

            //var link = data.Items[0].ExternalUrls.spotify; //АДРЕСА url ТРЕКУ
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

        public static string GetTopGenres(string all_genres_string)
        {
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
            //TODO: return string look better, do stuff with slashes

        }
    }
}