using Discord;
using Discord.Commands;
using Discord.Interactions;
using discord_bot.Common;
using discord_bot.Services;
using static System.Net.WebRequestMethods;
using RunMode = Discord.Interactions.RunMode;


namespace discord_bot.Modules;

public class SlashCommandsExample : InteractionModuleBase<ShardedInteractionContext>
{
    public InteractionService Commands { get; set; }

    private InteractionHandler _handler;

    public SlashCommandsExample(InteractionHandler handler)
    {
        _handler = handler;
    }

    [SlashCommand("my-command", "hello this works", runMode: RunMode.Async)]
    public async Task MyCommand()
    {
        await Context.Interaction.RespondAsync($"Hej {Context.Interaction.User.Username}");
    }
}
