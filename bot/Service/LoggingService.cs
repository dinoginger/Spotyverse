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
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        public LoggingService(IServiceProvider services)
        { 
            // get the services we need via DI, and assign the fields declared above to them
            _client = services.GetRequiredService<DiscordSocketClient>(); 
            _commands = services.GetRequiredService<CommandService>();
            _logger = services.GetRequiredService<ILogger<LoggingService>>(); //<ILogger<LoggingService>>(); means just type change

            // hook into these events with the methods provided below
            _client.Ready += OnReadyAsync;
            _client.Log += OnLogAsync;
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