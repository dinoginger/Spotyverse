using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Preconditions;
using Discord.Commands;
using Discord.WebSocket;
using Discord_API1.Service;
using Swan;


namespace Discord_API1.Modules
{
     
     public class Listen : ModuleBase<SocketCommandContext>
    {
        private struct song_data
        {
            public string songname;
            public int popularity; 
            public string genre_string;
        } 
        
        ///Default value of minutes command !listen runs, if not overloaded with minutes parameter
        private const int default_time = 5; //--minutes.

        private const int command_cooldown = 5;//--minutes \\\\Determines cooldown for our listen command;
        private const string command_cooldownSTR = "5";//for errormessage, duplicate 
        
        private const int wait_seconds = 30; //--seconds \\\\\Period of time we wait before checking song again


        [Command("listen", RunMode = RunMode.Async)]
        [Ratelimit(1, command_cooldown, Measure.Minutes,
            ErrorMessage =
                "Sheesh.. :eyes: cooldown of this command is set to 5 minutes!")] //TODO: "Command listen was executed at 15.02.2021 21:10:34." \n change log message when exited with cooldown error
        public async Task Listen_default(SocketUser user = null)
        {

            if (user == null)
            {
                user = Context.User;
            }

            var embedBuilder = new EmbedBuilder();

            int p = default_time * 60 / wait_seconds; //how many times cycle will run
            song_data[] songData = new song_data[p];
            int d = 0;
            string[] artist = new string[p];
            string[] title = new string[p];
            if (user == null)
            {
                user = Context.User;
            }

            await Context.Channel.SendMessageAsync("starting...");
            for (int i = 0; i < p; i++)
            {
                var activities = user.Activities;
                foreach (var activity in activities) //тайпчек всіх активностей на спотіфай
                {
                    if (activity is SpotifyGame spot)
                    {
                        try
                        {
                            Console.WriteLine($"{d + 1} song recieved");
                            var tuple = SpotifyService.Search_song(spot.TrackTitle + " " + spot.Artists.First());
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

                if (i != p - 1) //Для того, щоб після останньої пісні вже не робив ділей
                {
                    await Task.Delay(wait_seconds * 1000);
                }
            }


            Console.WriteLine("\n\n");
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
                distinct_genres = distinct_genres + "+" + data.genre_string;
            }

            Console.WriteLine($"\n\n{distinct_genres}");
            var topGenres = SpotifyService.GetTopGenres(distinct_genres);
            try
            {
                embedBuilder.WithAuthor($"for {user.Username}")
                    .WithTitle($"How basic your music taste is, based on {popularities.Length} songs :")
                    .AddField("============", $"Your playlist is {Math.Round(popularities.Average(), 1)}% basic.",
                        false)
                    .WithCurrentTimestamp()
                    .WithColor(Color.Purple);
                if (topGenres.Length > 2)
                {
                    embedBuilder.AddField("============", $"Top genres are : {topGenres}", false);
                }
                else
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












        [Command("listen", RunMode = RunMode.Async)]
        [Ratelimit(1,5,Measure.Minutes, RatelimitFlags.None)]
        public async Task Listen_overload1(float minutes, SocketUser user = null) //Перегруз де юзер задає скільки часу він хоче щоб його слухали
        {
            var embedBuilder = new EmbedBuilder();

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
            if (user == null)
            {
                user = Context.User;
            }

            await Context.Channel.SendMessageAsync("starting...");
            for (int i = 0; i < p; i++)
            {
                var activities = user.Activities;
                foreach (var activity in activities) //тайпчек всіх активностей на спотіфай
                {
                    if (activity is SpotifyGame spot)
                    {
                        try
                        {
                            Console.WriteLine($"{d + 1} song recieved");
                            var tuple = SpotifyService.Search_song(spot.TrackTitle + " " + spot.Artists.First());
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

                if (i != p - 1) //Для того, щоб після останньої пісні вже не робив ділей
                {
                    await Task.Delay(wait_seconds*1000);
                }
            }
            
            
            Console.WriteLine("\n\n");
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
            
            Console.WriteLine($"\n\n{distinct_genres}");
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
                else
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