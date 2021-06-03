using System.Threading.Tasks;
using Discord;
using Discord.Commands;


namespace SpotifyBot.Modules
{
    public class Test : ModuleBase<SocketCommandContext>
    {

        //Links to bot's pages on top.gg and discordbotlist.com
        private const string topgg_link = "https://top.gg/bot/802544166860095489/vote";
        private const string discordbotlist_link = "https://discordbotlist.com/bots/spotyverse/upvote";

        [Command("invite")]
        [Alias("inv")]
        public async Task invite()
        {
            await Context.Channel.SendMessageAsync("Sent you a DM's :smile:");
            Discord.IDMChannel gencom = await Context.Message.Author.GetOrCreateDMChannelAsync();
            var embed = new EmbedBuilder();
            embed.Description += $"Invite the bot to your server by [this link]({Program.invite_link})";
            embed.WithColor(new Color(125,164,120));
            var topgg = new EmbedFieldBuilder();
            topgg.IsInline = true;
            topgg.Name = "We on top.gg";
            topgg.Value = $"[vote!]({topgg_link})";
            embed.AddField(topgg);
            
            var dbotlist = new EmbedFieldBuilder();
            dbotlist.IsInline = true;
            dbotlist.Name = "We on discordbotlist";
            dbotlist.Value = $"[vote!]({discordbotlist_link})";
            embed.AddField(dbotlist);
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