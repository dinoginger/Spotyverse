using System;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
namespace Discord_API1
{
    public class Program
    {
        static void Main(string[] args)
            => new Program().StartAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;

        private async Task StartAsync()
        {
            _client = new DiscordSocketClient();
            await _client.LoginAsync(TokenType.Bot, "ODAyNTQ0MTY2ODYwMDk1NDg5.YAwxfw.1FANeB9IAtVFHhHpmQRAd-zkdaU");

            await _client.StartAsync(); //?????
            var _handler = new CommandHandler(_client);
            await Task.Delay(-1);
        }
    }
}