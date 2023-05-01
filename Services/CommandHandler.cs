using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using discord_bot.Common;

namespace discord_bot.Services;
public class CommandHandler
{
    private readonly DiscordShardedClient _client;
    private readonly CommandService _commands;

    public CommandHandler(
        DiscordShardedClient client,
        CommandService commands)
    {
        _client = client;
        _commands = commands;
    }

    public async Task InitializeAsync()
    {
        // add the public modules that inherit InteractionModuleBase<T> to the InteractionService
        //await _commands.AddModulesAsync(Assembly.GetExecutingAssembly(), Bootstrapper.ServiceProvider);

        // Subscribe a handler to see if a message invokes a command.
        _client.MessageReceived += HandleCommandAsync;

        _commands.CommandExecuted += async (optional, context, result) =>
        {
            if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
            {
                // the command failed, let's notify the user that something happened.
                await context.Channel.SendMessageAsync($"error: {result}");
            }
        };

        foreach (var module in _commands.Modules)
        {
            await Logger.Log(LogSeverity.Info, $"{nameof(CommandHandler)} | Commands", message: $"Module '{module.Name}' initialized.");
        }
    }

    private async Task HandleCommandAsync(SocketMessage arg)
    {
        // Bail out if it's a System Message.
        if (arg is not SocketUserMessage msg)
            return;

        // We don't want the bot to respond to itself or other bots.
        if (msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot)
            return;

        // Create a Command Context.
        var context = new ShardedCommandContext(_client, msg);

        var markPos = 0;
        if (context.Message.HasCharPrefix('!', ref markPos))
        {
            //await _commands.ExecuteAsync(context, markPos, Bootstrapper.ServiceProvider);
        }
    }
}