using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using discord_bot.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;

namespace discord_bot.Services;
public class InteractionHandler
{
    private readonly DiscordShardedClient _client;
    private readonly InteractionService _handler;
    private readonly IServiceProvider _services;
    private readonly IConfiguration _configuration;

    public InteractionHandler(DiscordShardedClient client, InteractionService handler, IServiceProvider services, IConfiguration config)
    {
        _client = client;
        _handler = handler;
        _services = services;
        _configuration = config;
    }

    public async Task InitializeAsync()
    {
        // add the public modules that inherit InteractionModuleBase<T> to the InteractionService
        await _handler.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        
        // Subscribe a handler to see if a message invokes a command.
        _client.InteractionCreated += HandleInteraction;
        _client.ShardReady += async shard =>
        {
            await ReadyAsync(shard);
        };

        _handler.InteractionExecuted += async (optional, context, result) =>
        {
            if (!result.IsSuccess && result.Error != InteractionCommandError.UnmetPrecondition)
            {
                // the command failed, let's notify the user that something happened.
                await context.Channel.SendMessageAsync($"error: {result}");
            }
        };


        foreach (var module in _handler.Modules)
        {
            await Logger.Log(LogSeverity.Info, $"{nameof(CommandHandler)} | Interactions", message: $"Module '{module.Name}' initialized.");
        }
    }
    private async Task ReadyAsync(DiscordSocketClient shard)
    {
        await Logger.Log(LogSeverity.Info, "ShardReady", $"Shard Number {shard.ShardId} is connected and ready!");
        // Context & Slash commands can be automatically registered, but this process needs to happen after the client enters the READY state.
        await _handler.RegisterCommandsGloballyAsync(deleteMissing: true);
    }

    private async Task HandleInteraction(SocketInteraction interaction)
    {

        try
        {
            // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules.
            var context = new ShardedInteractionContext(_client, interaction);

            // Execute the incoming command.
            var result = await _handler.ExecuteCommandAsync(context, _services);

        }
        catch
        {
            // If Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
            // response, or at least let the user know that something went wrong during the command execution.
            if (interaction.Type is InteractionType.ApplicationCommand)
                await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
        }
    }
}