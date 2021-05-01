using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace SpotifyBot.Other
{
    /// <summary>
    /// This class is for displaying better looking response to discord user.
    /// </summary>
    public class UserErrorResponses
    {
        private const string listen_response =
            " :robot: Hey, this command's input parameters are `<<listen [minutes]` or `<<listen [minutes] [@mention]`\nFor full command list type `<<help`";

        private const string search_response = " :robot: Hey, this command's input parameter is `<<search [search request]`\nFor full command list type `<<help`";
        public static async Task BadArg_Parse_Response(Optional<CommandInfo> command, ICommandContext context)
        {
            if (command.Value.Name == "listen")
            {
                EmbedBuilder embed = new EmbedBuilder();
                embed.WithDescription(listen_response);
                embed.Color = Color.Gold;
                await context.Channel.SendMessageAsync("", false, embed.Build());
            }
            else if (command.Value.Name == "search")
            {
                EmbedBuilder embed = new EmbedBuilder();
                embed.WithDescription(search_response);
                embed.Color = Color.Gold;
                await context.Channel.SendMessageAsync("", false, embed.Build());
            }
        }
    }
}