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
            "Hey, this commands input parameters are `<<listen [minutes]` or `<<listen [minutes] [@mention]`";

        private const string search_response = "Hey, this commands input parameter is `<<search <search request>`";
        public static async Task BadArg_Parse_Response(Optional<CommandInfo> command, ICommandContext context)
        {
            if (command.Value.Name == "listen")
            {
                await context.Channel.SendMessageAsync(listen_response);
            }
            else if (command.Value.Name == "search")
            {
                await context.Channel.SendMessageAsync(search_response);
            }
        }
    }
}