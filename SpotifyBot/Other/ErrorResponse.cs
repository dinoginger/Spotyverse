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
        public static async Task ListenErrorResponder(ICommandContext context, IResult result)
        {
            if (result.Error.Value == CommandError.ParseFailed)
            {
                await context.Channel.SendMessageAsync("Hey, this commands input parameters are `!listen <number>` or `!listen <number> @user_mention`");

            }
            else
            {
                await context.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
    }
}