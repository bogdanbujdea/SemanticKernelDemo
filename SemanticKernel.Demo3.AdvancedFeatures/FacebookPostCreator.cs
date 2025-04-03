using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace SemanticKernel.Demo3.AdvancedFeatures;

public class FacebookPostCreator
{
    private readonly ILogger<FacebookPostCreator> _logger;

    public FacebookPostCreator(ILogger<FacebookPostCreator> logger)
    {
        _logger = logger;
    }

    [KernelFunction, Description("Creates content for Facebook")]
    [return: Description("A json with a list of objects that contain the text for the Facebook post and the date when it should be scheduled")]
    [Experimental("SKEXP0010")]
    public async Task<string> GenerateFacebookContent(
        Kernel kernel,
        [Description("The topic for the posts")] string topic,
        [Description("The start date for the first post")] DateTime startDate
    )
    {
        _logger.LogInformation("Creating posts for topic {topic} starting on {startDate}", topic, startDate);
        var settings = new OpenAIPromptExecutionSettings
        {
            Temperature = 0.1,
            MaxTokens = 2000,
            ResponseFormat = "json_object"
        };
        var systemMessage = $$"""
                              You're an AI assistant who is specialized in marketing.
                              You will receive a topic and you will create Facebook content based on it, then you'll return a schedule with 5 posts for that content for the following 5 days.
                              Take into my account that my followers are from Romania, so the date for the post should be when most of them are using Facebook.
                              Use different time for each post.
                              Your response will be a json like this:
                              [
                                  {
                                      "post": "This is the Facebook post for day 1",
                                      "date": "2025-04-02T09:00:00Z"
                                  }
                              ]
                              You will then ask the user if they approve the posts, and you'll schedule them on Facebook using the tools at your disposal.
                              The start date is {{startDate:F}}

                              NEVER respond with plain text. ALWAYS use the JSON format specified above.
                              The topic is {{topic}}
                              """;
        var response = await kernel.InvokePromptAsync(systemMessage, new(settings));
        var json = response.GetValue<string>()!;
        _logger.LogInformation("JSON for Facebook posts: {json}", json);
        return json;
    }
}