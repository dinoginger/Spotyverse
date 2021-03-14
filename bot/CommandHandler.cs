﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using SpotifyBot.Other;
using SpotifyBot.Service;


namespace SpotifyBot
{
    public class CommandHandler
    
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;
        
        public CommandHandler(IServiceProvider services)
        {
            _services = services;
            _client = services.GetRequiredService<DiscordSocketClient>();
            _commands = services.GetRequiredService<CommandService>();
            
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
                if (result.Error.Value == CommandError.ParseFailed || result.Error.Value == CommandError.BadArgCount)
                {
                    await ErrorResponse.BadArg_Parse_Response(command, context);
                }
                
                else
                {
                    await context.Channel.SendMessageAsync(result.ErrorReason);
                }

                var commandName = command.IsSpecified ? command.Value.Name : "A command";
                Console.WriteLine($"Command {commandName} was failed to execute at {DateTime.UtcNow}. {result.Error.ToString()}: {result.ErrorReason}");
                
                //This is run for cooldown issue.
                var a = _services.GetService<_CooldownFixer>();
                a.wasSuccess = false;


            }
            else
            {
                var commandName = command.IsSpecified ? command.Value.Name : "A command";
                Console.WriteLine($"Command {commandName} was executed at {DateTime.UtcNow}.");
               
            }

            
            
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
            await _commands.ExecuteAsync(context, argPos, _services);
        }
    }
    
}