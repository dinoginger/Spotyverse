using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Preconditions;
using Discord.Commands;
using Discord.WebSocket;
using SpotifyBot.Spotify;
using SpotifyBot.Other;
using Swan;


namespace SpotifyBot.Modules
{
    //TODO: Services. telegram saved convo
     
     public class Listen : ModuleBase<SocketCommandContext>
    {
        private struct song_data
        {
            public string songname;
            public int popularity; 
            public string genre_string;
        } 
        
        ///Default value of minutes command !listen runs, if not overloaded with minutes parameter
        private const int command_cooldown = 5;//--minutes \\\\Determines cooldown for our listen command;
        
        private const int wait_seconds = 30; //--seconds \\\\\Period of time we wait before checking song again

        



        [Command("listen", RunMode = RunMode.Async)]
        [Ratelimit(1,5,Measure.Minutes, RatelimitFlags.None)] //My cooldown attr
        [Priority(1)] //less prioritized
        public async Task Listenn(float minutes)
        {
            var embedBuilder = new EmbedBuilder();
            SocketUser user = Context.User;
            if (minutes <= 0.5f)
            {
                await Context.Channel.SendMessageAsync("Time period must be more than 0,5 min.");
                return;
            }

            int p = (int)(minutes*60)/wait_seconds; //How many times cycle will run
            song_data[] songData = new song_data[p];
            int d = 0;
            string[] artist = new string[p];
            string[] title = new string[p];
            

            await Context.Channel.SendMessageAsync("starting...");
            for (int i = 0; i < p; i++)
            {
                var activities = user.Activities;
                foreach (var activity in activities) //Typecheckkk
                {
                    if (activity is SpotifyGame spot)
                    {
                        try
                        {
                            Console.WriteLine($"{d + 1} song recieved");
                            var tuple = SpotifyService.ListenSearch(spot.TrackTitle + " " + spot.Artists.First());
                            songData[d].popularity = tuple.Result.Item1;
                            songData[d].genre_string = tuple.Result.Item2;
                            
                            songData[d].songname = spot.TrackTitle;
                            Console.WriteLine(spot.TrackTitle);
                            Console.WriteLine(songData[d].genre_string);
                            d++;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.InnerException.Message);
                        }
                    }
                }

                if (i != p - 1) //For not delaying after last time
                {
                    await Task.Delay(wait_seconds*1000);
                }
            }
            
            
            //Console.WriteLine("\n\n");
            
            var distinct_data = songData.Distinct();
            int[] popularities = new int[distinct_data.Count()];
            int dd = 0;
            string distinct_genres = "";
            foreach (var data in distinct_data)
            {
                Console.WriteLine(data.songname);
                Console.WriteLine(data.genre_string);
                popularities[dd] = data.popularity;
                dd++;
                distinct_genres = distinct_genres + "+" +  data.genre_string;
            }

            //Console.WriteLine($"\n\n{distinct_genres}");
            
            var topGenres = SpotifyService.GetTopGenres(distinct_genres);
            try
            {
                embedBuilder.WithAuthor($"for {user.Username}")
                    .WithTitle($"How basic your music taste is, based on {popularities.Length} songs :")
                    .AddField("============", $"Your playlist is {Math.Round(popularities.Average(), 1)}% basic.", false)
                    .WithCurrentTimestamp()
                    .WithColor(Color.Purple);
                if (topGenres.Length > 2)
                {
                    embedBuilder.AddField("============", $"Top genres are : {topGenres}", false);
                }
                else //Funne face when no topgenres
                {
                    embedBuilder.AddField("============", "^_^", false);
                }
                await Context.Channel.SendMessageAsync("", false, embedBuilder.Build()); 
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                
                
            }

        }
        /// <summary>
        /// This overload is for custom user
        /// </summary>
        /// <param name="minutes"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [Command("listen", RunMode = RunMode.Async)]
        [MyRatelimit(1,5,Measure.Minutes, RatelimitFlags.None)] //default cooldown attr, or else it wont work
        [Priority(2)] //prioritized
        public async Task Listenn(float minutes, SocketUser user) //Перегруз де юзер задає скільки часу він хоче щоб його слухали
        {
            Console.Write("SECOND OVERLOAD WITH CUSTOM USER" + user.Username); 
            var embedBuilder = new EmbedBuilder();
            if (minutes <= 0.5f)
            {
                await Context.Channel.SendMessageAsync("Time period must be more than 0,5 min.");
                return;
            }

            int p = (int)(minutes*60)/wait_seconds; //How many times loop will run
            song_data[] songData = new song_data[p];
            int d = 0;
            string[] artist = new string[p];
            string[] title = new string[p];

            await Context.Channel.SendMessageAsync("starting...");
            for (int i = 0; i < p; i++)
            {
                var activities = user.Activities;
                foreach (var activity in activities) //Typecheck....
                {
                    if (activity is SpotifyGame spot)
                    {
                        try
                        {
                            Console.WriteLine($"{d + 1} song recieved");
                            var tuple = SpotifyService.ListenSearch(spot.TrackTitle + " " + spot.Artists.First());
                            songData[d].popularity = tuple.Result.Item1;
                            songData[d].genre_string = tuple.Result.Item2;
                            
                            songData[d].songname = spot.TrackTitle;
                            Console.WriteLine(spot.TrackTitle);
                            Console.WriteLine(songData[d].genre_string);
                            d++;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.InnerException.Message);
                        }
                    }
                }

                if (i != p - 1) //For not delaying after last time
                {
                    await Task.Delay(wait_seconds*1000);
                }
            }
            
            
            //Console.WriteLine("\n\n");
            
            var distinct_data = songData.Distinct();
            int[] popularities = new int[distinct_data.Count()];
            int dd = 0;
            string distinct_genres = "";
            foreach (var data in distinct_data)
            {
                Console.WriteLine(data.songname);
                Console.WriteLine(data.genre_string);
                popularities[dd] = data.popularity;
                dd++;
                distinct_genres = distinct_genres + "+" +  data.genre_string;
            }
            
            //Console.WriteLine($"\n\n{distinct_genres}"); - in case you wanna see it
            
            var topGenres = SpotifyService.GetTopGenres(distinct_genres);
            try
            {
                embedBuilder.WithAuthor($"for {user.Username}")
                    .WithTitle($"How basic your music taste is, based on {popularities.Length} songs :")
                    .AddField("============", $"Your playlist is {Math.Round(popularities.Average(), 1)}% basic.", false)
                    .WithCurrentTimestamp()
                    .WithColor(Color.Purple);
                if (topGenres.Length > 2)
                {
                    embedBuilder.AddField("============", $"Top genres are : {topGenres}", false);
                }
                else //Do funne face when no topgenres
                {
                    embedBuilder.AddField("============", "^_^", false);
                }
                await Context.Channel.SendMessageAsync("", false, embedBuilder.Build()); 
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
     
     