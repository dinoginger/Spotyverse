using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SpotifyBot.Service
{
    public class LoggingService
    {
        // declare the fields used later in this class
        private readonly ILogger _logger;
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;

        public LoggingService(IServiceProvider services)
        { 
            // get the services we need via DI, and assign the fields declared above to them
            _discord = services.GetRequiredService<DiscordSocketClient>(); 
            _commands = services.GetRequiredService<CommandService>();
            _logger = (ILogger)services.GetRequiredService<LoggingService>();

            // hook into these events with the methods provided below
            _discord.Ready += OnReadyAsync;
            _discord.Log += OnLogAsync;
            _commands.Log += OnLogAsync;
        }

        public Task OnReadyAsync()
        {
            return Task.CompletedTask;
        }
        
        
        public Task OnLogAsync(LogMessage msg)
        {
            return Task.CompletedTask;
        }
    }
}