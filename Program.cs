
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using discord_bot.Common;
using discord_bot.Init;
using discord_bot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using System.Linq;

internal class Program
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _services;

    private readonly DiscordSocketConfig _socketConfig = new()
    {
        LogLevel = LogSeverity.Info
    };
    private readonly InteractionServiceConfig _interactionServiceConfig = new()
    {
        LogLevel = LogSeverity.Info
    };
    private readonly CommandServiceConfig _commandServiceConfig = new()
    {
        LogLevel = LogSeverity.Info,
        CaseSensitiveCommands = false
    };

    public Program()
    {
        _configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        _services = new ServiceCollection()
            .AddSingleton(_configuration)
            .AddSingleton(_socketConfig)
            .AddSingleton(_interactionServiceConfig)
            .AddSingleton(_commandServiceConfig)
            .AddSingleton<DiscordShardedClient>()
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordShardedClient>()))
            //.AddSingleton<CommandHandler>()
            .AddSingleton<InteractionHandler>()
            .BuildServiceProvider();
    }
    static void Main(string[] args)
            => new Program().RunAsync()
                .GetAwaiter()
                .GetResult();

    public async Task RunAsync()
    {
        var client = _services.GetRequiredService<DiscordShardedClient>();

        client.Log += Logger.Log;

        // Here we can initialize the service that will register and execute our commands
        //await _services.GetRequiredService<ICommandHandler>().InitializeAsync();
        await _services.GetRequiredService<InteractionHandler>().InitializeAsync();


        // Bot token can be provided from the Configuration object we set up earlier
        await client.LoginAsync(TokenType.Bot, _configuration.GetRequiredSection("Settings")["DiscordBotToken"]);
        await client.StartAsync();

        // Never quit the program until manually forced to.
        await Task.Delay(Timeout.Infinite);
    }
}
