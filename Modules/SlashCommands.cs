using Azure;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using discord_bot.Common;
using discord_bot.Models;
using discord_bot.Services;
using static System.Net.WebRequestMethods;
using RunMode = Discord.Interactions.RunMode;


namespace discord_bot.Modules;

public class SlashCommandsExample : InteractionModuleBase<ShardedInteractionContext>
{
    public InteractionService Commands { get; set; }

    private InteractionHandler _handler;
    private ChatService _chatService;
    private Session? CurrentSession { get; set; }
    public static List<Session> ChatSessions { get; set; } = new();

    public SlashCommandsExample(InteractionHandler handler, ChatService chatService)
    {
        _handler = handler;
        _chatService = chatService;
    }

    [SlashCommand("ask-bot", "Ask the OpenAI bot a question", runMode: RunMode.Async)]
    public async Task AskBot(string msg)
    {
        if (string.IsNullOrEmpty(msg))
        {
            return;
        }
        await Context.Interaction.DeferAsync();
        await Context.Interaction.ModifyOriginalResponseAsync(async updateResponse =>
        {
            updateResponse.Content = await GetResponse(msg);
        });
    }
    private async Task<string> GetResponse(string msg)
    {
        var user = Context.Interaction.User;

        ChatSessions = await _chatService.GetAllChatSessionsAsync();
        var CurrentSession = ChatSessions.Where(x => x.SessionId == user.Id.ToString());
        if (CurrentSession.Count() == 0)
        {
            await _chatService.CreateNewChatSessionAsync(user.Id.ToString());
        }
        else
        {
            await Logger.Log(LogSeverity.Info, "ask-bot", CurrentSession.Max(x => x.TokensUsed).ToString());
        }
        return await _chatService.GetChatCompletionAsync(user.Id.ToString(), msg);
    }
}
