using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Discord_API1.Modules
{
    public class Test : ModuleBase<SocketCommandContext>
    {
      
        [Command("Test")]
        public async Task UserInfo(SocketUser user = null)
        {
            if (user == null)
            {
                user = Context.User;
            }

            var activities = user.Activities;
            foreach (var activity in activities)
            {
                if (activity is SpotifyGame spot)
                {
                    await Context.Channel.SendMessageAsync($"You are currently listening to {spot.TrackTitle} by {spot.Artists.First()}");
                }
            }
        }

        [Command("help")]
        public async Task help()
        { 
            var a = new Discord.EmbedBuilder();
            a.WithTitle("Commands");
            a.WithDescription("General Commands\n-- .help // Gives list of commands to use");
            Discord.IDMChannel gencom = await Context.Message.Author.GetOrCreateDMChannelAsync();
            await gencom.SendMessageAsync("lmao noob get rekt ", false);
            await gencom.CloseAsync();
        }

        [Command("long", RunMode = RunMode.Async)]
        public async Task long_comm(SocketUser user = null)
        {
            if (user == null)
            {
                user = Context.User;
            }

            var mention = user.Mention.ToString();
            await Context.Channel.SendMessageAsync($"Long task - starting *{mention}*");
            await Task.Delay(150000); // = 2,5 хвилини
            await Context.Channel.SendMessageAsync($"Long task - 50% (2.5 mins to go) **{mention}**");
            await Task.Delay(150000);
            await Context.Channel.SendMessageAsync($"Long task - 100%. finished ***{mention}***");
        }

        [Command("listen", RunMode = RunMode.Async)]
        public async Task runmode(SocketUser user = null)
        {
            int n = 3; //скільки разів пісню чекаємо?
            string[] title = new string[n];
            string[] artist = new string[n];
            if (user == null)
            {
                user = Context.User;
            }

            await Context.Channel.SendMessageAsync("starting...");
            for (int i = 0; i < n; i++)
            {
                var activities = user.Activities; 
                foreach (var activity in activities) //тайпчек всіх активностей на спотіфай
                {
                    if (activity is SpotifyGame spot)
                    {
                        title[i] = spot.TrackTitle;
                        artist[i] = spot.Artists.First();
                        Console.WriteLine($"{i+1} song recieved");
                    }
                }

                if (i != n-1){
                    await Task.Delay(20000);
                }
            }

            title = title.Distinct().ToArray(); //видалення дублікованих пісень


            for (int i = 0; i <  title.Length; i++)
            {
                await Context.Channel.SendMessageAsync($"{title[i]} by {artist[i]}");
            }
        }
    }
}