using Discord.Commands;

namespace SpotifyBot.Other
{
    public class MyCommandResult : RuntimeResult
    {

        //Creating own CommandResult to abort command better.
        public MyCommandResult(CommandError? error, string reason) : base(error, reason)
        {
        }
        public static MyCommandResult FromError(string reason) =>
            new MyCommandResult(CommandError.Unsuccessful, reason);
        public static MyCommandResult FromSuccess(string reason = null) =>
            new MyCommandResult(null, reason);
    }
}