using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord_API1.Service;

namespace Discord_API1.Modules
{
    public class Test : ModuleBase<SocketCommandContext>
    {

        [Command("Test")]
        public async Task UserInfo(SocketUser user = null)
        {
            if (user == null)
            {
                user = Context.User;
            }

            var activities = user.Activities;
            foreach (var activity in activities)
            {
                if (activity is SpotifyGame spot)
                {
                    await Context.Channel.SendMessageAsync(
                        $"You are currently listening to {spot.TrackTitle} by {spot.Artists.First()}");
                }
            }
        }

        [Command("help")]
        public async Task help()
        {
            var a = new Discord.EmbedBuilder();
            a.WithTitle("Commands");
            a.WithDescription("General Commands\n-- .help // Gives list of commands to use");
            Discord.IDMChannel gencom = await Context.Message.Author.GetOrCreateDMChannelAsync();
            await gencom.SendMessageAsync("lmao noob get rekt ", false);
            await gencom.CloseAsync();
        }

        [Command("long", RunMode = RunMode.Async)]
        public async Task long_comm(SocketUser user = null)
        {
            if (user == null)
            {
                user = Context.User;
            }

            var mention = user.Mention.ToString();
            await Context.Channel.SendMessageAsync($"Long task - starting *{mention}*");
            await Task.Delay(150000); // = 2,5 хвилини
            await Context.Channel.SendMessageAsync($"Long task - 50% (2.5 mins to go) **{mention}**");
            await Task.Delay(150000);
            await Context.Channel.SendMessageAsync($"Long task - 100%. finished ***{mention}***");
        }

        [Command("listen", RunMode = RunMode.Async)]
        public async Task runmode(SocketUser user = null)
        {
            var embedBuilder = new EmbedBuilder(); //запихаємо все в красивий ембед
            int n = 3; //скільки разів пісню прочекуємо?
            string[] title = new string[n];
            string[] artist = new string[n];
            if (user == null)
            {
                user = Context.User;
            }

            await Context.Channel.SendMessageAsync("starting...");
            for (int i = 0; i < n; i++)
            {
                var activities = user.Activities;
                foreach (var activity in activities) //тайпчек всіх активностей на спотіфай
                {
                    if (activity is SpotifyGame spot)
                    {
                        title[i] = spot.TrackTitle;
                        artist[i] = spot.Artists.First();
                        Console.WriteLine($"{i + 1} song recieved");
                    }
                }

                if (i != n - 1) //Для того, щоб після останньої пісні вже не робив ділей
                {
                    await Task.Delay(5000);
                }
            }

            title = title.Distinct().ToArray(); //видалення дублікованих пісень
            embedBuilder.WithAuthor($"for : {user.Username}") //автор 
                .WithTitle("List of songs you listened to :") //Титулка
                .WithCurrentTimestamp() //часова марка, каррент коли була відіслана
                .WithColor(Color.Green);

            
            for (int i = 0; i < title.Length; i++)
            {
                if (title[i] != null)
                {
                    embedBuilder.AddField("", $"{title[i]} - {artist[i]}", inline: false); //Додаємо поля, інлайн фолс бо якщо тру то робляться колонки
                }
            }
            await Context.Channel.SendMessageAsync("", false, embedBuilder.Build()); //висилаємо ембед
        }

        ///ЗРОБИТИ ВСЕ ЦЕ В СЕРВІСІ А НЕ В ТАСКУ
        [Command("Search")]
        public async Task search([Remainder] string msg)
        {
            
            if (msg != null)
            {
                Console.WriteLine("we in!");
                try
                {
                    var tuple = SpotifyService.Search_song(msg);
                    await Context.Channel.SendMessageAsync($"Spotify link to song is ({tuple.Result.Item2})");
                    await Context.Channel.SendMessageAsync($"Spotify song popularity is {tuple.Result.Item1}");
                }
                catch (Exception e)
                {
                    Console.WriteLine("we messed up");
                    await Context.Channel.SendMessageAsync($"{e.InnerException.Message}");
                    //throw;
                }
                

            }
            else
            {
                Console.WriteLine("msg is null");
            }
            
        }
    }
}