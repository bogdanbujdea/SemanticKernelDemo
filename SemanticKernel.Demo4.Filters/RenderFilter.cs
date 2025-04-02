using Microsoft.SemanticKernel;

namespace SemanticKernel.Demo3.AdvancedFeatures;

public class RenderFilter: IPromptRenderFilter
{
    public async Task OnPromptRenderAsync(PromptRenderContext context, Func<PromptRenderContext, Task> next)
    {
        var functionName = context.Function.Name;

        await next(context);

        Console.WriteLine($"Function is {functionName} and rendered prompt is {context.RenderedPrompt}");
    }
}