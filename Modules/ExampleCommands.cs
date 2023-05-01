using Discord;
using Discord.Commands;
using Discord.Interactions;
using discord_bot.Common;
using discord_bot.Services;
using static System.Net.WebRequestMethods;
using RunMode = Discord.Commands.RunMode;

namespace discord_bot.Modules;

public class ExampleCommands : ModuleBase<ShardedCommandContext>
{
    public CommandService CommandService { get; set; }

    [Command("hello", RunMode = RunMode.Async)]
    public async Task Hello()
    {
        await Context.Message.ReplyAsync($"Hello {Context.User.Username}. Nice to meet you!");
    }
    [Command("bye", RunMode = RunMode.Async)]
    public async Task Bye()
    {
        await Logger.Log(LogSeverity.Info, $"{nameof(CommandHandler)} | bye handler", message: $"Message: {Context.Message}");

        await Context.Message.ReplyAsync($"Goodbye {Context.User.Username}!!");
    }

}