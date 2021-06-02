using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Preconditions;
using Discord.Commands;
using Discord.WebSocket;
using SpotifyBot.Service.Spotify;
using Microsoft.Extensions.DependencyInjection;
using SpotifyBot.Other;
using SpotifyBot.Service.ForCooldown;
using Swan;


namespace SpotifyBot.Modules
{
    public class ListenCommand : ModuleBase<SocketCommandContext>
    {
        private struct song_data
        {
            public int s_popularity;
            public int a_popularity;
            public string genre_string;
        }

        private const string spotify_activity_Explanation = "https://top.gg/bot/802544166860095489#spotify";
        private const int wait_seconds = 30; //--seconds \\\\\Period of time we wait before checking song again

        private SpotifyService spotify;
        private ListenUsersList _listenUsersList;
        private Dictionary<ulong, Tuple<TimeSpan,DateTime>> _dictionary;

        public ListenCommand(IServiceProvider serviceProvider)
        {
            spotify = serviceProvider.GetRequiredService<SpotifyService>();
            _listenUsersList = serviceProvider.GetRequiredService<ListenUsersList>();
            _dictionary= _listenUsersList._UsrDict;
        }
        
        /// <summary>
        /// Listen command description
        /// </summary>
        /// <param name="minutes"></param>
        /// <param name="user"></param>
        /// <returns></returns>

        [Command("listen", RunMode = RunMode.Async)]
        public async Task<RuntimeResult> Listenn(float minutes,SocketUser user = null)
        {
            if (user == null)
            {
                user = Context.User;
            }

            DateTime utcNow = DateTime.UtcNow;
            if (_dictionary.ContainsKey(user.Id))
            {
                return MyCommandResult.FromError($"Shhh... :eyes: 'Listen' is already running for user {user.Mention}!\nEstimated end time : `{(_dictionary[user.Id].Item1 - (utcNow - _dictionary[user.Id].Item2) ).ToString(@"hh\:mm\:ss")}`");
            }
            else
            {
                TimeSpan span = TimeSpan.FromMinutes(minutes); // converting float minutes to timespan minutes
                _dictionary.Add(user.Id, new Tuple<TimeSpan, DateTime>(span, utcNow)); //filling dict
            }


            Random random = new Random();
            Color color = new Color(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
            try
            {
                Console.Write(user.Username + "\n"); //test : see which user we listen to 
                var embedBuilder = new EmbedBuilder();
                if (minutes < 0.5f)
                {
                    throw new ArgumentException("Time period must be more than 0,5 min.");
                }
                if (minutes > 60f)
                {
                    throw new ArgumentException("Time period cant be more than 1 hour.");
                }

                int p = (int) (minutes * 60) / wait_seconds; //How many times loop will run
                song_data[] songData = new song_data[p];
                int d = 0;
                var embed = new EmbedBuilder();
                embed.Description += $"starting... \nListening for {minutes} minute(s)..";
                embed.WithColor(color);
                await Context.Channel.SendMessageAsync("", false, embed.Build());
                for (int i = 0; i < p; i++)
                {
                    bool Spotify_exists  = false; //To determine whether first activity check contains spotify 
                    var activities = user.Activities;
                    foreach (var activity in activities) //Typecheck....
                    {
                        if (activity is SpotifyGame spot)
                        {
                            Spotify_exists = true;
                            //Console.WriteLine($"{d + 1} song recieved");
                            var tuple = spotify.Listen(spot.TrackTitle + " " + spot.Artists.First());
                            songData[d].s_popularity = tuple.Result.Item1; // <- song pop
                            songData[d].a_popularity = tuple.Result.Item2; // <- artist pop
                            songData[d].genre_string = tuple.Result.Item3;
                            //Console.WriteLine(spot.TrackTitle);
                            //Console.WriteLine(songData[d].genre_string);
                            d++;
                        }
                    }

                    if (!Spotify_exists && i == 0)
                    {
                        throw new Exception($"No spotify activity detected.\n[What is spotify activity?]({spotify_activity_Explanation}) :confused:");
                    }

                    if (i != p - 1) //For not delaying after last time
                    {
                        await Task.Delay(wait_seconds * 1000);
                    }
                }


                //Console.WriteLine("\n\n");

                var distinct_data = songData.Distinct(); //Deletes similar elements.
                float[] popularities = new float[distinct_data.Count()];// song popularities
                int dd = 0;
                string distinct_genres = "";
                foreach (var data in distinct_data)
                {
                    //Console.WriteLine(data.songname);
                    //Console.WriteLine(data.genre_string);
                    popularities[dd] = (data.s_popularity*0.25f + data.a_popularity*0.75f);
                    dd++;
                    distinct_genres = distinct_genres + "+" + data.genre_string;//all genres to one string, in order to pass it to GetTopGenres();
                }

                //Console.WriteLine($"\n\n{distinct_genres}"); - in case you wanna see it

                var topGenres = spotify.GetTopGenres(distinct_genres);
                var field = new EmbedFieldBuilder();
                
                field.WithName($"How basic your music taste is, based on {popularities.Length} song(s) ");
                field.WithValue($"Your current songs are `{Math.Round(popularities.Average(), 1)}%` basic.");
                field.IsInline = true;
                

                var author = new EmbedAuthorBuilder();
                author.Name += $"for {user.Username}";
                author.IconUrl += user.GetAvatarUrl();
                
                embedBuilder.AddField(field);
                embedBuilder.WithAuthor(author)
                    .WithCurrentTimestamp()
                    .WithColor(color)
                    .WithThumbnailUrl("https://upload.wikimedia.org/wikipedia/commons/thumb/1/19/Spotify_logo_without_text.svg/768px-Spotify_logo_without_text.svg.png");
                if (topGenres.Length > 2)
                {
                    var genre_field = new EmbedFieldBuilder();
                    genre_field.WithName("**Top genres are** :");
                    genre_field.IsInline = true;

                    var genre_arr = topGenres.Split(", ");
                    foreach (var genre in genre_arr)
                    {
                        genre_field.Value += $"• {genre}\n";
                    }
                    embedBuilder.AddField(genre_field);
                }

                RemoveUser(user,_dictionary);
                await Context.Channel.SendMessageAsync(user.Mention, false, embedBuilder.Build());
                return MyCommandResult.FromSuccess();
            }
            catch (Exception e)
            {

                RemoveUser(user, _dictionary);
                return MyCommandResult.FromError($"Command aborted : {e.Message}");
            }
        }

        //Removes element from dict, thats all
        private void RemoveUser(SocketUser user, Dictionary<ulong, Tuple<TimeSpan,DateTime>> _dict)
        {
            _dict.Remove(user.Id);
        }
    }
}

     
     