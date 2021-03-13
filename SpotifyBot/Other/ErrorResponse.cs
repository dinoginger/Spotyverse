using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace SpotifyBot.Other
{
    /// <summary>
    /// This class is for displaying better looking response to discord user.
    /// </summary>
    public class ErrorResponse
    {
        
        public static async Task BadArg_Parse_Response(Optional<CommandInfo> command, ICommandContext context)
        {
            if (command.Value.Name == "listen")
            {
                await context.Channel.SendMessageAsync("Hey, this commands input parameters are `!listen <number>` or `!listen <number> @user_mention`");
            }
            else if (command.Value.Name == "search")
            {
                await context.Channel.SendMessageAsync("Hey, this commands input parameter is `!search <search request>`");
            }
        }
    }
}