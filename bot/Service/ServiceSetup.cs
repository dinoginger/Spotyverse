using System;
using Discord.Commands;
using Discord.WebSocket;
using SpotifyBot.Service.Spotify;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace SpotifyBot.Service
{
    public class service
    {
        private readonly DiscordSocketClient _client;
        private _CooldownFixer _cooldownFixer = new _CooldownFixer();

        public static ServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton(typeof(CommandService),
                    new CommandService(new CommandServiceConfig
                    {
                        IgnoreExtraArgs = true,
                        CaseSensitiveCommands = false,
                    }))
                .AddSingleton<_CooldownFixer>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<LoggingService>()
                .AddSingleton<SpotifyService>()
                .AddLogging(configure => configure.AddSerilog()); //Registering ILogger to use in any other injected service element

            
            //Setting up log level if it was given in Program.cs;
            if (!string.IsNullOrEmpty(Program._logLevel)) 
            {
                switch (Program._logLevel.ToLower())
                {
                    case "info":
                    {
                        services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information);
                        break;
                    }
                    case "error":
                    {
                        services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Error);
                        break;
                    } 
                    case "debug":
                    {
                        services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Debug);
                        break;
                    } 
                    default: 
                    {
                        services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information);
                        break;
                    }
                }
            }
            else
            {
                services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information);
            }

            
            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }
    }
}