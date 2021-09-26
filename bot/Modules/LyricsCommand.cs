using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Interactivity;
using Interactivity.Pagination;
using SpotifyBot.Other;
using SpotifyBot.Service;

namespace SpotifyBot.Modules
{
    public class LyricsCommand : ModuleBase<SocketCommandContext>
    {
        private const string spotify_activity_Explanation = "https://top.gg/bot/802544166860095489#spotify";

        public InteractivityService Interactivity { get; set; }

        
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
                        //item1 - lyrics, item2 - image url
                        var results = LyricsScrapper.Get(new string(spot.Artists.First() + " " + spot.TrackTitle));
                        
                        //building  pieces for embeds/pages
                        Random random = new Random();
                        var footer = new EmbedFooterBuilder();
                        footer.Text += $"for {user.Username}";
                        footer.IconUrl += user.GetAvatarUrl();
                        Color color = new Color(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
                        string[] lyrics = SplitString(results.Item1, 2048);
                        foreach (var VARIABLE in lyrics)
                        {
                            Console.WriteLine(VARIABLE);
                            Console.WriteLine("\n\n\n");
                        }
                        
                        //Building paginator part
                        //timeout embed
                        EmbedBuilder timeoutembed = new EmbedBuilder();
                        timeoutembed.WithFooter(footer);
                        timeoutembed.WithThumbnailUrl(results.Item2);
                        timeoutembed.WithColor(color);
                        timeoutembed.WithDescription(lyrics[0]);
                        
                        //pages
                        List<PageBuilder> pages = new List<PageBuilder>();
                            foreach (string part in lyrics)
                        {
                            pages.Add(new PageBuilder().WithFooter(footer).WithThumbnailUrl(results.Item2)
                                .WithColor(color).WithDescription(part));
                        }
                        
                        
                        var paginator = new StaticPaginatorBuilder()
                            .WithTimoutedEmbed(timeoutembed)
                            .WithUsers(Context.User)
                            .WithPages(pages)
                            .WithFooter(PaginatorFooter.PageNumber | PaginatorFooter.Users)
                            .WithDefaultEmotes()
                            .Build();


                        await Interactivity.SendPaginatorAsync(paginator, Context.Channel, TimeSpan.FromMinutes(2));
                        //await Context.Channel.SendMessageAsync(results.Item1,false, embed.Build());
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
/// <summary>
/// Is used to split lyrics string into smaller strings, so that it wont exceed 2048 limit which is put on description in embed. 
/// </summary>
/// <param name="lyrics"></param>
/// <returns></returns>
public static string[] SplitString(string input, int lineLen) 
{
            StringBuilder sb = new StringBuilder();
            string[] words = input.Split(' ');
            string line = string.Empty;
            string sp = string.Empty;
            foreach (string w in words)
            {
                string word = w;
                while (word != string.Empty)
                {
                    if (line == string.Empty)
                    {
                        while (word.Length >= lineLen)
                        {
                            sb.Append(word.Substring(0, lineLen) + "~");
                            word = word.Substring(lineLen);
                        }
                        if (word != string.Empty)
                            line = word;
                        word = string.Empty;
                        sp = " ";
                    }
                    else if (line.Length + word.Length <= lineLen)
                    {
                        line += sp + word;
                        sp = " ";
                        word = string.Empty;
                    }
                    else
                    {
                        sb.Append(line + "~");
                        line = string.Empty;
                        sp = string.Empty;
                    }
                }
            }
            if (line != string.Empty)
                sb.Append(line);
            return sb.ToString().Split('~');
         }
    }
}