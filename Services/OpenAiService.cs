using Azure;
using Azure.AI.OpenAI;
using discord_bot.Models;

namespace discord_bot.Services;
public class OpenAiService
{
    private readonly string _deploymentName = String.Empty;
    private readonly int _maxConversationTokens = default;
    private readonly OpenAIClient _client;


    //System prompt to send with user prompts to instruct the model for chat session
    private readonly string _systemPrompt = @"
    You are an AI assistant that helps people find information.
    Provide concise answers that are polite and professional.
    
    You are responding to a discord chat, use discord styntaxt to highligt messages, code blocks, emojis etc" + Environment.NewLine;


    //System prompt to send with user prompts to instruct the model for summarization
    private readonly string _summarizePrompt = @"
    Summarize this prompt in one or two words to use as a label in a button on a web page" + Environment.NewLine;


    /// <summary>
    /// Gets the maximum number of tokens to limit chat conversation length.
    /// </summary>
    public int MaxConversationTokens
    {
        get => _maxConversationTokens;
    }

    /// <summary>
    /// Creates a new instance of the service.
    /// </summary>
    /// <param name="key">Account key.</param>
    /// <param name="deploymentName">Name of the deployment access.</param>
    /// <param name="maxTokens">Maximum number of tokens per request.</param>
    /// <exception cref="ArgumentNullException">Thrown when endpoint, key, deploymentName, or maxTokens is either null or empty.</exception>
    /// <remarks>
    /// This constructor will validate credentials and create a HTTP client instance.
    /// </remarks>
    public OpenAiService(string key, string deploymentName, string maxConversationTokens)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNullOrEmpty(deploymentName);
        ArgumentNullException.ThrowIfNullOrEmpty(maxConversationTokens);

        _deploymentName = deploymentName;
        _maxConversationTokens = Int32.TryParse(maxConversationTokens, out _maxConversationTokens) ? _maxConversationTokens : 3000;

        _client = new(key, new OpenAIClientOptions());
    }

    /// <summary>
    /// Sends a prompt to the deployed OpenAI LLM model and returns the response.
    /// </summary>
    /// <param name="sessionId">Chat session identifier for the current conversation.</param>
    /// <param name="prompt">Prompt message to send to the deployment.</param>
    /// <returns>Response from the OpenAI model along with tokens for the prompt and response.</returns>
    public async Task<(string response, int promptTokens, int responseTokens)> GetChatCompletionAsync(string sessionId, string userPrompt)
    {

        ChatMessage systemMessage = new(ChatRole.System, _systemPrompt);
        ChatMessage userMessage = new(ChatRole.User, userPrompt);

        ChatCompletionsOptions options = new()
        {

            Messages =
        {
            systemMessage,
            userMessage
        },
            User = sessionId,
            MaxTokens = 1000,
            Temperature = 0.3f,
            NucleusSamplingFactor = 0.5f,
            FrequencyPenalty = 0,
            PresencePenalty = 0
        };

        Response<ChatCompletions> completionsResponse = await _client.GetChatCompletionsAsync(_deploymentName, options);


        ChatCompletions completions = completionsResponse.Value;

        return (
            response: completions.Choices[0].Message.Content,
            promptTokens: completions.Usage.PromptTokens,
            responseTokens: completions.Usage.CompletionTokens
        );
    }
}
