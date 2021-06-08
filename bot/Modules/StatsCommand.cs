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
                await spotifyService.Auth();
                await Context.Channel.SendMessageAsync(spotifyService._request.ToUri().ToString());
                Thread.Sleep(10000);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return MyCommandResult.FromError(e.Message);
            }

            
            
                Context.Channel.SendMessageAsync(
                    $"Hi, {spotifyService._client.UserProfile.Current().Result.DisplayName}!");
            

            return MyCommandResult.FromSuccess();
        }
    }
}