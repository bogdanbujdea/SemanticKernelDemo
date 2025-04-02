using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.ComponentModel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

var kernelBuilder = Kernel
    .CreateBuilder()
    .AddOpenAIChatCompletion(modelId: "gpt-4o-mini", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY")!);

var kernel = kernelBuilder.Build();

// Import the letter counter plugin
kernel.ImportPluginFromType<LetterCounterPlugin>();

var chatHistory = new ChatHistory();
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
chatHistory.AddSystemMessage("You are an AI that can count letters, use the tools at your disposal to do this");
var executionSettings = new OpenAIPromptExecutionSettings
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

while (true)
{
    Console.Write("\nYou: ");
    var userInput = Console.ReadLine();

    chatHistory.AddUserMessage(userInput!);

    var response = await chatCompletionService.GetChatMessageContentAsync(chatHistory, executionSettings, kernel);
    chatHistory.AddAssistantMessage(response.Content!);

    Console.Write($"\nAssistant: {response.Content}\n");
}

public class LetterCounterPlugin
{
    [KernelFunction, Description("Counts the number of letters in a given word (case insensitive)")]
    public int CountLetters(
        [Description("The counted letter")] char letter,
        [Description("The word to count letters in")] string word
    )
    {
        var count = word.Count(c => char.ToLower(c) == char.ToLower(letter));
        return count;
    }
}