using System;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;


namespace SpotifyBot.Service
{
    public class Initialize
    {
        private readonly DiscordSocketClient _client;
        private _CooldownFixer _cooldownFixer = new _CooldownFixer();

        public Initialize(DiscordSocketClient client = null)
        {
           
            _client = client ?? new DiscordSocketClient();
        }

        public IServiceProvider BuildServiceProvider() => new ServiceCollection()
            .AddSingleton(_client)
            .AddSingleton(typeof(CommandService), 
                new CommandService(new CommandServiceConfig {
                    IgnoreExtraArgs = true,
                    CaseSensitiveCommands = false,
                    //LogLevel = LogSeverity.Verbose
                }))
            .AddSingleton<_CooldownFixer>()
            .BuildServiceProvider();
        
            
    }
}