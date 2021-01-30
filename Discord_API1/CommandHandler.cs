using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;

namespace Discord_API1
{
    public class CommandHandler
    
    {
        private DiscordSocketClient _client;
        private CommandService _service;
        
        public CommandHandler(DiscordSocketClient client)
        { 
            _client = client;
            _service = new CommandService();
            _client.MessageReceived += HandleCommandAsync;
            _service.AddModulesAsync(Assembly.GetEntryAssembly(), null);
        }
        private async Task HandleCommandAsync(SocketMessage message)
        {
            var msg = message as SocketUserMessage;
            if (msg ==null) return;
            int argPos = 0;
            var _context = new SocketCommandContext(_client,msg);
            if(msg.HasCharPrefix('!',ref argPos))
            {

                var result = await _service.ExecuteAsync(_context, argPos, services: null);
                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    await _context.Channel.SendMessageAsync(result.ErrorReason);
                    
                }
            }
        }
    }
    
}