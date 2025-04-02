using Microsoft.SemanticKernel;

namespace SemanticKernel.Demo3.AdvancedFeatures;

public class UserPermissionFilter : IFunctionInvocationFilter
{
    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        if (context.Arguments.ContainsName("userId"))
        {
            var userId = context.Arguments["userId"]?.ToString();
            if (userId!.Length == 0)
            {
                throw new Exception("This user is not allowed to post on Facebook. Inform the user that they must send an email to admin@company.com and request this permission.");
            }
        }
        await next(context);
    }
}