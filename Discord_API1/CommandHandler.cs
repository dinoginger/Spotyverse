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
        private CommandService _commands;
        
        public CommandHandler(DiscordSocketClient client)
        { 
            _client = client;
            _commands = new CommandService();
            _commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);
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
            if (!string.IsNullOrEmpty(result?.ErrorReason)) //TODO: попроацювати над ерор хендлером більше, після пулу TimeOut бранчу допрацювати і з ним теж
            {
                await context.Channel.SendMessageAsync(result.ErrorReason);
            }

            // ...or even log the result (the method used should fit into
            // your existing log handler)
            var commandName = command.IsSpecified ? command.Value.Name : "A command";
            Console.WriteLine($"Command {commandName} was executed at {DateTime.UtcNow}.");
            
        }
        public async Task HandleCommandAsync(SocketMessage msg)
        {
            var message = msg as SocketUserMessage;
            if (message == null) return;
            int argPos = 0;
            if (!(message.HasCharPrefix('!', ref argPos) ||
                  message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
            {
                return;
            }
            
            var context = new SocketCommandContext(_client, message);
            await _commands.ExecuteAsync(context, argPos, null);
        }
    }
    
}