using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace discord_bot.Options;
public record CosmosDb
{
    public required string Endpoint { get; init; }

    public required string Key { get; init; }

    public required string Database { get; init; }

    public required string Container { get; init; }
};
