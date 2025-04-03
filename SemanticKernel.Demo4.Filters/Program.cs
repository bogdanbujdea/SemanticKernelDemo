using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SemanticKernel.Demo3.AdvancedFeatures;

// Configure logging
var services = new ServiceCollection();
services.AddLogging(builder =>
{
    builder.SetMinimumLevel(LogLevel.Trace).AddConsole();
});

var serviceProvider = services.BuildServiceProvider();
var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
var logger = loggerFactory.CreateLogger<Program>();

var kernelBuilder = Kernel
    .CreateBuilder()
    .AddOpenAIChatCompletion(modelId: "gpt-4o-mini", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY")!);

var kernel = kernelBuilder.Build();

kernel.ImportPluginFromObject(
    new FacebookPostScheduler(loggerFactory.CreateLogger<FacebookPostScheduler>()), "FacebookPostScheduler"
);
kernel.ImportPluginFromObject(
    new FacebookPostCreator(loggerFactory.CreateLogger<FacebookPostCreator>()), "FacebookPostCreator"
);

kernel.FunctionInvocationFilters.Add(new UserPermissionFilter());
kernel.PromptRenderFilters.Add(new RenderFilter());

var chatHistory = new ChatHistory();
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

var executionSettings = new OpenAIPromptExecutionSettings
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
    Temperature = 0.1,
    ModelId = "gpt-4o-mini",
    MaxTokens = 2000
};
var systemMessage = $$"""
                              You're an AI assistant who is specialized in marketing.
                              You will help the user generate Facebook posts and schedule them on Facebook.
                              Always ask for confirmation before scheduling posts.
                              The current date in UTC time is {{DateTime.UtcNow:F}}
                              The id of the user is 1234
                              """;

chatHistory.AddSystemMessage(
    systemMessage
);
Console.WriteLine("Starting Semantic Kernel social media planner");

while (true)
{
    Console.Write("\nYou: ");
    var userInput = Console.ReadLine();

    if (string.IsNullOrEmpty(userInput) || userInput.ToLower() == "exit")
        break;

    chatHistory.AddUserMessage(userInput);
    var response = await chatCompletionService.GetChatMessageContentAsync(chatHistory,
        executionSettings,
        kernel);

    Console.Write($"\nAssistant: {response.Content}");
    logger.LogTrace("Assistant response: {Response}", JsonSerializer.Serialize(response.Metadata!["Usage"]));
    chatHistory.AddAssistantMessage(response.Content!);
}

logger.LogInformation("Chat session ended");
