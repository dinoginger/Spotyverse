using System;
using System.IO;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using Discord.Commands;
using SpotifyBot.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using LogLevel = Swan.Logging.LogLevel;

namespace SpotifyBot
{
    
    public class Program
    {
        
        public static string _logLevel;
        static void Main(string[] args = null)
        {
            
            //setting log level if run specialised, else with information;
            if (args.Length > 0)
            {
                _logLevel = args[0];
            }
            
            /*
            Write out to the console and to a file

            The file will be stored in the logs directory (from the project root), and will create a new file named csharpi.log 

            The files will roll every day, and have each day’s date as a timestamp (Serilog takes care of this magic)
             */
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(@"C:\Users\Марко\OneDrive\Desktop\Discord\TestBotStuff\TestBot_\bot\logs\myk.log",rollingInterval: RollingInterval.Day) //New log file is created every day.
                .WriteTo.Console()
                .MinimumLevel.Information()
                .CreateLogger();
            
            
            
            new Program().MainAsync().GetAwaiter().GetResult();
        }




        private readonly string configPath = @"C:\Users\Марко\OneDrive\Desktop\Discord\TestBotStuff\TestBot_\bot\_config.json";
        private string bot_token;
        public static string invite_link;
        
        private DiscordSocketClient _client;

        private CommandService _commands;

        private async Task MainAsync()
        {
            using (var services = service.BuildServiceProvider()) //(Comments for me) using in brackets mean declaring and redeclaring stuff??;
            {
                // get the client and assign to client 
                // you get the services via GetRequiredService<T>
                var client = services.GetRequiredService<DiscordSocketClient>();
                _client = client;
                
                // setup logging and the ready event
                services.GetRequiredService<LoggingService>();
                
                
                bot_token = GetToken();
                await _client.LoginAsync(TokenType.Bot, bot_token);
                await _client.SetGameAsync("your music | !help", null, ActivityType.Listening);
                await _client.StartAsync(); //?????
                _commands = services.GetService<CommandService>();

                //Init Command handler from services.
                var handler = services.GetRequiredService<CommandHandler>();// <--- starts running on innit
                await Task.Delay(-1);
            }
        }

        private string GetToken()
        {
            StreamReader r = new StreamReader(configPath);
            string json = r.ReadToEnd();
            dynamic config_file = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            invite_link = config_file.Invite_link;
            return config_file.TestBot_token;
        }
    }
}