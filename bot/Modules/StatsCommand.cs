using System;
using System.Threading;
using System.Threading.Tasks;
using Discord.Commands;
using SpotifyBot.Other;
using SpotifyBot.Service.Spotify;

namespace SpotifyBot.Modules
{
    public class StatsCommand : ModuleBase<SocketCommandContext>
    {
        [Command("auth")]
        public async Task<RuntimeResult> auth()
        {
            SpotifyService spotifyService = new SpotifyService();
            try
            {
                await Context.Channel.SendMessageAsync("Authorize [here](http://localhost:5000/callback)");
                await spotifyService.Auth();
                Thread.Sleep(30);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return MyCommandResult.FromError(e.Message);
            }

            if (spotifyService._client != null)
            {
                Context.Channel.SendMessageAsync(
                    $"Hi, {spotifyService._client.UserProfile.Current().Result.DisplayName}!");
            }

            return MyCommandResult.FromSuccess();
        }
    }
}