using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace SemanticKernel.Demo3.AdvancedFeatures;

public class FacebookPostScheduler
{
    private readonly ILogger<FacebookPostScheduler> _logger;

    public FacebookPostScheduler(ILogger<FacebookPostScheduler> logger)
    {
        _logger = logger;
    }

    [KernelFunction, Description("Posts a message on Facebook")]
    [return: Description("Returns true for success, false otherwise")]
    public async Task<bool> ScheduleFacebookPost(
        [Description("The text for the post")] string facebookPost,
        [Description("The date when the post should be scheduled")] DateTime dateTime,
        [Description("The userId")] string userId
    )
    {
        _logger.LogInformation("Scheduled post {post} on: {date}", facebookPost, dateTime);
        return true;
    }
}