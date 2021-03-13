using System;
using System.IO;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using Discord.Commands;
using SpotifyBot.Service;
using Microsoft.Extensions.DependencyInjection;
using Swan;

namespace SpotifyBot
{
    public class Program
    {
        static void Main(string[] args)
            => new Program().StartAsync().GetAwaiter().GetResult();


        private readonly string configPath = @"C:\Users\Марко\OneDrive\Desktop\Discord\TestBotStuff\TestBot_\bot\_config.json";
        private string bot_token;
        
        private DiscordSocketClient _client;

        private CommandService _commands;

        private async Task StartAsync()
        {
            _client = new DiscordSocketClient();
            bot_token = GetToken();
            await _client.LoginAsync(TokenType.Bot, bot_token);

            await _client.StartAsync(); //?????
            var Service_init  = new Initialize(_client);
            var service_provider = Service_init.BuildServiceProvider();
            _commands = service_provider.GetService<CommandService>();
            var _handler = new CommandHandler(service_provider, _commands, _client); 
            await Task.Delay(-1);
        }

        private string GetToken()
        {
            StreamReader r = new StreamReader(configPath);
            string json = r.ReadToEnd();
            dynamic token = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            return token.TestBot_token;
        }
    }
}