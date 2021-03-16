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
        public async Task invite()
        {
            Discord.IDMChannel gencom = await Context.Message.Author.GetOrCreateDMChannelAsync();
            await gencom.SendMessageAsync($"Here is my link invite : \n{Program.invite_link}", false);
            await gencom.CloseAsync(); 
        }

        [Command("help")]
        public async Task help()
        {
            var a = new Discord.EmbedBuilder();
            a.WithTitle("Commands");
            a.WithDescription("``<<help`` - Gives list of commands to use" +
                              "\n``<<invite`` - Sends you bot invite link in DMs" +
                              "\n``<<search [your request]`` - Information about artist/album/track" +
                              "\n``<<listen [minutes]`` - Listens to your Spotify activity and gives statistics" +
                              "\n``<<listen [minutes] [@mention]`` - Listens to custom user's Spotify activity");
            a.WithColor(Color.Magenta);
            await Context.Channel.SendMessageAsync("", false, a.Build());
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