using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord_API1.Service;

namespace Discord_API1.Modules
{
    public struct song_data
    {
        public string songname;
        public int popularity;
    }
    public class Listen : ModuleBase<SocketCommandContext>
    {
        ///Default value of minutes command !listen runs, if not overloaded with minutes parameter
        private const int default_time = 5; //--minutes.
        
        private const int wait_seconds = 30; //--seconds \\\\\Period of time we wait before checking song again
        
        
        
        [Command("listen", RunMode = RunMode.Async)]
        public async Task listen(SocketUser user = null)
        {
            var embedBuilder = new EmbedBuilder();

            int p = default_time*60/wait_seconds; //how many times cycle will run
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
                            var tuple = SpotifyService.Search_song(spot.TrackTitle);
                            songData[d].popularity = tuple.Result.Item1;
                            songData[d].songname = spot.TrackTitle;
                            Console.WriteLine(spot.TrackTitle);
                            Console.WriteLine($"{d + 1} song recieved");
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
            
            Console.WriteLine("\n");
            
            foreach (var data in songData)
            {
                Console.WriteLine(data.songname);
                Console.WriteLine(data.popularity);
            }
            Console.WriteLine("\n\n");
            var a = songData.Distinct();
            int[] popularities = new int[a.Count()];
            int dd = 0;
            foreach (var data in a)
            {
                Console.WriteLine(data.songname);
                Console.WriteLine(data.popularity);
                popularities[dd] = data.popularity;
            }

            try
            {
                embedBuilder.WithAuthor($"for {user.Username}")
                    .WithTitle($"How basic your music taste is, based on {popularities.Length} songs :")
                    .AddField("============", $"Your playlist is {Math.Round(popularities.Average(), 2)}% basic.", false)
                    .AddField("============", " :) ",false) 
                    .WithCurrentTimestamp()
                    .WithColor(Color.Purple);
                await Context.Channel.SendMessageAsync("", false, embedBuilder.Build()); 
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                
            }
           
        }

        
        
        
        
        
        
        
        
        
        
        
        [Command("listen", RunMode = RunMode.Async)]
        public async Task listen(float minutes, SocketUser user = null) //Перегруз де юзер задає скільки часу він хоче щоб його слухали
        
        {
            var embedBuilder = new EmbedBuilder();

            if (minutes <= 0.5f)
            {
                Context.Channel.SendMessageAsync("Time period must be more than 0,5 min.");
                return;
            }

            int p = (int)minutes*60/wait_seconds; //How many times cycle will run
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
                            var tuple = SpotifyService.Search_song(spot.TrackTitle);
                            songData[d].popularity = tuple.Result.Item1;
                            songData[d].songname = spot.TrackTitle;
                            Console.WriteLine(spot.TrackTitle);
                            Console.WriteLine($"{d + 1} song recieved");
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
            
            Console.WriteLine("\n");
            
            foreach (var data in songData)
            {
                Console.WriteLine(data.songname);
                Console.WriteLine(data.popularity);
            }
            Console.WriteLine("\n\n");
            var a = songData.Distinct();
            int[] popularities = new int[a.Count()];
            int dd = 0;
            foreach (var data in a)
            {
                Console.WriteLine(data.songname);
                Console.WriteLine(data.popularity);
                popularities[dd] = data.popularity;
            }

            try
            {
                embedBuilder.WithAuthor($"for {user.Username}")
                    .WithTitle($"How basic your music taste is, based on {popularities.Length} songs :")
                    .AddField("============", $"Your playlist is {Math.Round(popularities.Average(), 2)}% basic.", false)
                    .AddField("============", " :) ",false) 
                    .WithCurrentTimestamp()
                    .WithColor(Color.Purple);
                await Context.Channel.SendMessageAsync("", false, embedBuilder.Build()); 
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                
            }
           
        }
    }
}