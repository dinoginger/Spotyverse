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
            _logger = services
                .GetRequiredService<ILogger<LoggingService>
                >(); //<ILogger<LoggingService>>() injecting logger to logging service???

            // hook into these events with the methods provided below
            _client.Ready += OnReadyAsync;
            _client.Log += OnLogAsync;
            _commands.Log += OnLogAsync;

        }

        // this method executes on the bot being connected/ready
        public Task OnReadyAsync()
        {
            _logger.LogInformation($"Connected as -> [{_client.CurrentUser}] :");
            _logger.LogInformation($"We are on [{_client.Guilds.Count}] servers");
            return Task.CompletedTask;
        }

        // this method switches out the severity level from Discord.Net's API, and logs appropriately
        public async Task OnLogAsync(LogMessage msg)
        {
            string logText = $": {msg.Exception?.ToString() ?? msg.Message}";

            if (msg.Exception is CommandException cmdException)
            {
                // We can tell the user that something unexpected has happened
                await cmdException.Context.Channel.SendMessageAsync("Something went catastrophically wrong!");

                // We can also log this incident
                Console.WriteLine(
                    $"{cmdException.Context.User} failed to execute '{cmdException.Command.Name}' in {cmdException.Context.Channel}.");
            }

            switch (msg.Severity.ToString())
            {
                case "Critical":
                {
                    _logger.LogCritical(logText);
                    break;
                }
                case "Warning":
                {
                    _logger.LogWarning(logText);
                    break;
                }
                case "Info":
                {
                    _logger.LogInformation(logText);
                    break;
                }
                case "Verbose":
                {
                    _logger.LogInformation(logText);
                    break;
                }
                case "Debug":
                {
                    _logger.LogDebug(logText);
                    break;
                }
                case "Error":
                {
                    _logger.LogError(logText);
                    break;
                }

            }
        }
    }
}