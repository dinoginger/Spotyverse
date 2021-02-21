using System;
using System.IO;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using Discord.Commands;
using SpotifyBot.Service;
using Swan;

namespace SpotifyBot
{
    public class Program
    {
        static void Main(string[] args)
            => new Program().StartAsync().GetAwaiter().GetResult();


        private readonly string configPath = @"C:\Users\Марко\OneDrive\Desktop\Discord\TestBotStuff\TestBot_\SpotifyBot\_config.json";
        private string bot_token;
        
        private DiscordSocketClient _client;

        private CommandService _commands;

        private async Task StartAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            bot_token = GetToken();
            await _client.LoginAsync(TokenType.Bot, bot_token);

            await _client.StartAsync(); //?????
            var Service_init  = new Initialize(_commands, _client);
            var _handler = new CommandHandler(Service_init.BuildServiceProvider(), _commands, _client); 
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