using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using SpotifyAPI.Web;

namespace SpotifyBot.Service.Spotify
{
    public partial class SpotifyService
    {
        
        private static SocketUser user;
        private SpotifyClient spotify;
        public async Task<EmbedBuilder> Search(string requestString, SocketUser socketUser)
        {
            string song_name;
            string album_name;
            int popuarity;

            user = socketUser;
            string artistname;
            string genres_string = "";
            EmbedBuilder embedBuilder = new EmbedBuilder();

            //Getting tokens from our json.
            try
            {
                //Connection of Bot client
                var config = SpotifyClientConfig
                    .CreateDefault()
                    .WithAuthenticator(new ClientCredentialsAuthenticator(app_id, app_ids));
                spotify = new SpotifyClient(config);
            }
            catch (APIUnauthorizedException e)
            {
                Console.Write("problem api unathor");
                Console.Write("UNATHORIZED"+ e.Message + "with : " + e.Response?.StatusCode);
                throw new ApplicationException(e.Message + "with : " + e.Response?.StatusCode);
            }


            try
            {
                var result = spotify.Search.Item(new SearchRequest(SearchRequest.Types.All,requestString)); //Sending search request and obtaining data obj
                EmbedBuilder embed = EmbedCreator(result.Result, spotify);
                if (embed == null)
                {
                    throw new ArgumentException("Embed is zero");
                }

                return embed;


            }
            catch (APIException e)
            {
                Console.Write("problem api excep");
                Console.Write("api excheption"+ e.Message + "with : " + e.Response?.StatusCode);
                throw new ApplicationException(e.Message + "with : " + e.Response?.StatusCode);
                
            }
            catch (Exception e)
            {
                throw new ArgumentException($"No matches found for \"{requestString}\".");
            }
        }


        /// <summary>
        /// This is created to give to user already filtred search response.
        /// </summary>
        /// <param name="response"></param>
        private static EmbedBuilder EmbedCreator(SearchResponse response, SpotifyClient spotify)
        {
            try
            {
                EmbedBuilder embedBuilder = new EmbedBuilder();
                //So order of data checking is 
                //Artist - or Album - or Track;


                //===============checking if artist


                var artists = response.Artists.Items;
                if (artists.Count > 0)
                {
                    foreach (var artist in artists)
                    {
                        if (artist.Followers.Total > 1000)
                        {

                            //Building embed; 
                            
                            var footer = new EmbedFooterBuilder();
                            footer.Text += $"for {user.Username}";
                            footer.IconUrl += user.GetAvatarUrl();
                            
                            
                            embedBuilder.WithFooter(footer);

                            //artist field
                            var Artist_field = new EmbedFieldBuilder();
                            Artist_field.WithName("Artist : ");
                            Artist_field.WithValue($"Name: [{artist.Name}]({artist.ExternalUrls["spotify"]}) \n");
                            Artist_field.IsInline = false;
                            //if there are genres in array, add em.
                            if (artist.Genres.ToArray().Length != 0)
                            {
                                string genres_string = "";
                                List<string> list = new List<string>();
                                foreach (var genre in artist.Genres.ToArray())
                                {
                                    list.Add(genre);
                                }
                                
                                genres_string = String.Join(", ", list);
                                Artist_field.Value += $"Main genres : **{genres_string}**";
                            }

                            embedBuilder.AddField(Artist_field);

                            //Here adding url of image.
                            embedBuilder.ImageUrl = artist.Images[0].Url;


                            //Getting top tracks of artist
                            var result = spotify.Artists.GetTopTracks(artist.Id, new ArtistsTopTracksRequest("US"));

                            string toptracks = "";
                            int i = 1;
                            foreach (var track in result.Result.Tracks.ToArray())
                            {
                                toptracks += $"\n{i}. [{track.Name}]({track.ExternalUrls["spotify"]})";
                                i++;
                            }

                            embedBuilder.AddField("Top Tracks :", toptracks);

                            
                            return embedBuilder;

                        }
                    }
                }

                ///If its not an artist we check whether album by this request exists.

                if (response.Albums.Items.Count > 0 )
                {
                    var album = response.Albums.Items[0];
                    if (album.AlbumType != "single")
                    {
                        //Building embed; 
                        var footer = new EmbedFooterBuilder();
                        footer.Text += $"for {user.Username}";
                        footer.IconUrl += user.GetAvatarUrl();
                            
                            
                        embedBuilder.WithFooter(footer);

                        //album field
                        var Album_field = new EmbedFieldBuilder();
                        Album_field.WithName("Album : ");
                        Album_field.WithValue(
                            $"Name: [{album.Name}]({album.ExternalUrls["spotify"]}) \nArtist : [{album.Artists[0].Name}]({album.Artists[0].ExternalUrls["spotify"]})" +
                            $"\nAlbum type : **{album.AlbumType}**\nAlbum release date : **{album.ReleaseDate}**");
                        Album_field.IsInline = false;

                        //Here adding url of image.
                        embedBuilder.ImageUrl = album.Images[0].Url;

                        embedBuilder.AddField(Album_field);


                        return embedBuilder;
                    }
                }
                //..Our last hope - track with that name exists.

                if (response.Tracks.Items.Count > 0)
                {
                    var track = response.Tracks.Items[0];
                    
                    var footer = new EmbedFooterBuilder();
                    footer.Text += $"for {user.Username}";
                    footer.IconUrl += user.GetAvatarUrl();
                            
                            
                    embedBuilder.WithFooter(footer);
                    
                    
                    //album field
                    var track_field  = new EmbedFieldBuilder();
                    string track_album_name = ""; //Created bc we want to add prefix [single] if album is a single.
                    if (track.Album.AlbumType == "single")
                    {
                        track_album_name += "[single] ";
                    }

                    track_album_name += track.Album.Name;
                    
                    //
                    track_field.WithName("Track : ");
                    track_field.WithValue(
                        $"Name: [{track.Name}]({track.ExternalUrls["spotify"]}) " +
                        $"\nAlbum : [{track_album_name}]({track.Album.ExternalUrls["spotify"]})" +
                        $"\nAlbum release date : **{track.Album.ReleaseDate}**" + $"\nArtist : [{track.Artists[0].Name}]({track.Artists[0].ExternalUrls["spotify"]})");
                    track_field.IsInline = false;

                    //Here adding url of image.
                    embedBuilder.ImageUrl = track.Album.Images[0].Url;
                    embedBuilder.AddField(track_field);
                    return embedBuilder;

                }

                return null;
                
                
            }
            catch (Exception e)
            {
                Console.WriteLine("search_spotify method crashed.");
                throw;
            }
        }
    }
}
        

