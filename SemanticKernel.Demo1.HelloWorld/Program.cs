using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

var kernelBuilder = Kernel
    .CreateBuilder()
    .AddOpenAIChatCompletion(modelId: "gpt-4o-mini", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY")!);

var kernel = kernelBuilder.Build();

var chatHistory = new ChatHistory();
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

while (true)
{
    Console.Write("\nYou: ");
    var userInput = Console.ReadLine();

    chatHistory.AddUserMessage(userInput!);

    var response = await chatCompletionService.GetChatMessageContentAsync(chatHistory);
    chatHistory.AddAssistantMessage(response.Content!);

    Console.Write($"\nAssistant: {response.Content}\n");
}