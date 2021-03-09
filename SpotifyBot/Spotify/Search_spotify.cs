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
                var result = await spotify.Search.Item(new SearchRequest(SearchRequest.Types.All,
                        songName)); //Sending search request and obtaining data obj
                EmbedBuilder embed = EmbedCreator(result, spotify);
                return embed;

            }
            catch(Exception e)
            {
                throw new ArgumentException($"Song \"{songName}\" was not found. {e}");
                throw;
            }



            /*
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
                throw;
            }
            
            return embedBuilder;

            */
            return null;

        }

        
        /// <summary>
        /// This is created to give to user already filtred search response.
        /// </summary>
        /// <param name="response"></param>
        private static EmbedBuilder EmbedCreator(SearchResponse response, SpotifyClient spotify)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            //So order of data checking is 
            //Artist - or Album - or Track;
            
            
            //===============checking if artist
            var artists = response.Artists.Items;
            foreach (var artist in artists)
            {
                if (artist.Followers.Total > 1000)
                {
                    try
                    {
            
                        //Building embed; 
                        embedBuilder.WithTitle("Search results :");
                
                        //artist field
                        var Artist_field = new EmbedFieldBuilder();
                        Artist_field.WithName("Artist : ");
                        Artist_field.WithValue($"Name: [{artist.Name}]({artist.ExternalUrls["spotify"]}) \n");
                        Artist_field.IsInline = false;
                        //if there are genres in array, add em.
                        if (artist.Genres.ToArray().Length != 0)
                        {
                            string genres_string = "";
                            foreach (var genre in artist.Genres.ToArray())
                            {
                                genres_string = genres_string + "," + genre;
                            }
                            Artist_field.Value += $"Main genres : {genres_string}";
                        }

                        embedBuilder.AddField(Artist_field);
                        
                        //Here adding url of image.
                        embedBuilder.ImageUrl = artist.Images[0].Url ?? null;
                        
                        
                        //Getting top tracks of artist
                        var result = spotify.Artists.GetTopTracks(artist.Id, new ArtistsTopTracksRequest("US"));
                        
                        string toptracks = "";
                        int i = 1;
                        foreach (var track in result.Result.Tracks.ToArray())
                        {
                            toptracks += $"\n{i}. [{track.Name}]({track.ExternalUrls["spotify"]})";
                            i++;
                        }
                        embedBuilder.AddField("Top Tracks :", toptracks );

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("search_spotify method crashed.");
                        throw;
                    }
            
                    return embedBuilder;

                }
            }

            return null;
        }
        


    }
    
}

