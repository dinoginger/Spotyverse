using System;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using Discord.Commands;
using Discord_API1.Service;

namespace Discord_API1
{
    public class Program
    {
        static void Main(string[] args)
            => new Program().StartAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private async Task StartAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            await _client.LoginAsync(TokenType.Bot, "ODAyNTQ0MTY2ODYwMDk1NDg5.YAwxfw.PaDEkHOOVQ1CUUfchM3OyzlIouU");

            await _client.StartAsync(); //?????
            var Service_init  = new Initialize(_commands, _client);
            var _handler = new CommandHandler(Service_init.BuildServiceProvider(), _commands, _client); 
            await Task.Delay(-1);
        }
    }
}