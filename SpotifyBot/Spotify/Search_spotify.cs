using System;
using System.Threading.Tasks;
using Discord;
using SpotifyAPI.Web;
using Swan;

namespace SpotifyBot.Spotify
{
    public partial class SpotifyService
    {
        public static async Task<EmbedBuilder> Search(string songName)
        {
            string song_name;
            string album_name;
            int popuarity;

            string artistname;
            string genres_string = "";
            EmbedBuilder embedBuilder = new EmbedBuilder();
            
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
                //String : song name;
                song_name = data.Items[0].Name.ToString();

                //String : album name;
                album_name = data.Items[0].Album.Name.ToString();

                //Int : song popularity;
                popuarity = (int) data.Items[0].Popularity;

                
                //GenreSearch
                string artistid = data.Items[0].Album.Artists[0].Id.ToString();
                var artist = await spotify.Artists.Get(artistid); 
                //String : Artist name;
                artistname = data.Items[0].Artists[0].Name.ToString(); 
                
                //String : Genres;
                foreach (var genre in artist.Genres.ToArray())
                {
                    
                    genres_string = genres_string + ", " + genre;
                }
            }
            catch
            {
                throw new ArgumentException($"Song \"{songName}\" was not found.");
            }

            try
            {
                //Building embed; 
                embedBuilder.WithTitle("Search results :");
                
                //song field
                var Song_field = new EmbedFieldBuilder();
                Song_field.WithName("Song : ");
                Song_field.WithValue($"Name : {song_name ?? throw new Exception("")} \n");
                Song_field.Value += $"Album: {album_name ?? throw new Exception("")} \n";
                Song_field.Value += $"Popularity : {popuarity}";
                Song_field.IsInline = false;
                embedBuilder.AddField(Song_field);

                //artist field
                var Artist_field = new EmbedFieldBuilder();
                Artist_field.WithName("Artist : ");
                Artist_field.WithValue($"Name: {artistname ?? throw new Exception("")} \n");
                Artist_field.IsInline = false;
                if (!string.IsNullOrEmpty(genres_string))
                {
                    Artist_field.Value += $"Main genres : {genres_string}";
                }
                embedBuilder.AddField(Artist_field);
            }
            catch (Exception e)
            {
                Console.WriteLine("search_spotify method crashed. ");
            }
            
            return embedBuilder;


        }
    }
}