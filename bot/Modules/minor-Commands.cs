using System.Threading.Tasks;
using Discord;
using Discord.Commands;


namespace SpotifyBot.Modules
{
    public class Test : ModuleBase<SocketCommandContext>
    {

        [Command("invite")]
        [Alias("inv")]
        public async Task invite()
        {
            await Context.Channel.SendMessageAsync("Sent you a DM's :smile:");
            Discord.IDMChannel gencom = await Context.Message.Author.GetOrCreateDMChannelAsync();
            var embed = new EmbedBuilder();
            embed.Description += $"Invite the bot to your server by [this link]({Program.invite_link})";
            embed.WithColor(new Color(125,164,120));
            await gencom.SendMessageAsync("", false, embed.Build());
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
                              "\n``<<listen [# minutes]`` - Listens to your Spotify activity and gives statistics" +
                              "\n``<<listen [# minutes] [@mention]`` - Listens to custom user's Spotify activity");
            a.WithColor(Color.Magenta);
            await Context.Channel.SendMessageAsync("", false, a.Build());
        }
    }
}