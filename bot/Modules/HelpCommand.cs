using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace SpotifyBot.Modules
{
    public class HelpCommand : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        public async Task help()
        {
            var a = new Discord.EmbedBuilder();
            a.WithTitle("Commands");
            a.WithDescription("``<<help`` - Gives list of commands to use" +
                              "\n``<<invite`` - Sends you bot invite link in DMs" +
                              "\n``<<search [your request]`` - Information about artist/album/track" +
                              "\n``<<listen [# minutes]`` - Listens to your Spotify activity and gives statistics" +
                              "\n``<<listen [# minutes] [@mention]`` - Listens to custom user's Spotify activity");
            a.WithColor(Color.Magenta);
            await Context.Channel.SendMessageAsync("", false, a.Build());
        }
        
    }
}