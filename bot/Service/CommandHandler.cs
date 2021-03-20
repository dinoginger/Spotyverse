using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SpotifyBot.Other;


namespace SpotifyBot.Service
{
    public class CommandHandler
    
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;
        private ILogger _logger;
        
        public CommandHandler(IServiceProvider services)
        {
            _services = services;
            _client = services.GetRequiredService<DiscordSocketClient>();
            _commands = services.GetRequiredService<CommandService>();
            _logger = services.GetRequiredService<ILogger<CommandHandler>>();
            
            _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            // Hook the execution event
            _commands.CommandExecuted += OnCommandExecutedAsync;
            // Hook the command handler
            _client.MessageReceived += HandleCommandAsync;
        }
        public async Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // We have access to the information of the command executed,
            // the context of the command, and the result returned from the
            // execution in this event.

            // We can tell the user what went wrong 
            if (!string.IsNullOrEmpty(result?.ErrorReason))
            {
                if (result.Error.Value == CommandError.ParseFailed || result.Error.Value == CommandError.BadArgCount || result.Error.Value == CommandError.ObjectNotFound)
                {
                    await UserErrorResponses.BadArg_Parse_Response(command, context);
                }
                
                else
                {
                    if (result.Error != CommandError.UnknownCommand)
                    {
                        await context.Channel.SendMessageAsync(result.ErrorReason);
                    }
                    
                }

                var commandName = command.IsSpecified ? command.Value.Name : "A command";
                if (result.Error != CommandError.UnmetPrecondition && result.Error != CommandError.UnknownCommand) //Ignore ratelimits, they will occur a lot.
                {
                    _logger.LogError($"Command {commandName} was failed to execute for {context.User.Username}. {result.Error.ToString()}: {result.ErrorReason}");
                }
                
                
                //This is run for cooldown issue.
                var a = _services.GetService<_CooldownFixer>();
                a.wasSuccess = false;


            }
            /*
            else
            {
                var commandName = command.IsSpecified ? command.Value.Name : "A command";
                _logger.LogInformation($"Command {commandName} was executed for {context.User.Username}.");
               
            } Dont need logging of successful tasks atm.
            */

            
            
        }
        public async Task HandleCommandAsync(SocketMessage msg)
        {
            
            var message = msg as SocketUserMessage;
            if (message.Author.IsBot)
            {
                return;
            }
            if (message == null) return;
            int argPos = 0;
            if (!((message.HasStringPrefix("<<", ref argPos) || 
                   message.HasMentionPrefix(_client.CurrentUser, ref argPos))))
            {
                return;
            }
            
            var context = new SocketCommandContext(_client, message);
            await _commands.ExecuteAsync(context, argPos, _services);
        }
    }
    
}