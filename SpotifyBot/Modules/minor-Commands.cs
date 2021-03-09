using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Preconditions;
using Discord.Commands;
using Discord.WebSocket;
using SpotifyBot.Other;
using SpotifyBot.Spotify;


namespace SpotifyBot.Modules
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
                    await Context.Channel.SendMessageAsync(
                        $"You are currently listening to {spot.TrackTitle} by {spot.Artists.First()}");
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
        

        [Command("Search")]
        public async Task search([Remainder] string msg)
        {
            
            if (msg != null)
            {
                Console.WriteLine("we in!");
                try
                {
                    EmbedBuilder embedBuilerr = await SpotifyService.Search(msg);
                    await Context.Channel.SendMessageAsync("", false, embedBuilerr.Build());
                }
                catch (Exception e)
                {
                    Console.WriteLine("we messed up");
                    await Context.Channel.SendMessageAsync($"{e.Message}");
                    //throw;
                }
                

            }
            else
            {
                Console.WriteLine("msg is null");
            }
            
        }
    }
}