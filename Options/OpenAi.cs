using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace discord_bot.Options;
public record OpenAi
{
    public required string Key { get; init; }

    public required string Deployment { get; init; }

    public string? MaxConversationTokens { get; init; }
}