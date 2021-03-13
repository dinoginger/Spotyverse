using System;
using System.Linq;

using System.Threading.Tasks;
using Discord;

using Discord.Commands;
using Discord.WebSocket;
using SpotifyBot.Other;
using SpotifyBot.Spotify;


namespace SpotifyBot.Modules
{
    public class Test : ModuleBase<SocketCommandContext>
    {

        [Command("invite")]
        public async Task UserInfo()
        {
            await ReplyAsync($"Here is my link invite : {Program.invite_link}");
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
        

        [Command("search")]
        [MyRatelimit(3, 30,MyMeasure.Seconds)]
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
                    throw;
                }
                

            }
            else
            {
                Console.WriteLine("msg is null");
            }
            
        }
    }
}