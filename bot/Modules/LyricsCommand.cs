using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SpotifyBot.Other;
using SpotifyBot.Service;

namespace SpotifyBot.Modules
{
    public class LyricsCommand : ModuleBase<SocketCommandContext>
    {
        private const string spotify_activity_Explanation = "https://top.gg/bot/802544166860095489#spotify";

        
        [Command("lyrics")]
        public async Task<RuntimeResult> lyrics()
        {
            try
            {
                
                SocketUser user = Context.User;

                    var activities = user.Activities;
                bool Spotify_exists = false;

                foreach (var activity in activities) //Typecheck....
                {
                    if (activity is SpotifyGame spot)
                    {
                        Spotify_exists = true;
                        string lyrics = LyricsScrapper.Get(new string(spot.Artists.First() + " " + spot.TrackTitle));
                        await Context.Channel.SendMessageAsync(lyrics, false);
                        return MyCommandResult.FromSuccess();
                    }
                }

                if (!Spotify_exists)
                {
                    throw new Exception(
                        $"No spotify activity detected.\n[What is spotify activity?]({spotify_activity_Explanation}) :confused:");
                }

                return MyCommandResult.FromSuccess();
            }
            catch (Exception e)
            {
                return MyCommandResult.FromError(e.Message);
            }
        }
    }
}