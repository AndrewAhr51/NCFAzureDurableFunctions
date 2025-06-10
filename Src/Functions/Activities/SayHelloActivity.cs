using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace NCFAzureDurableFunctions.Src.Functions.Activities // ✅ Moved to Activities folder for clarity
{
    public static class SayHelloActivity
    {
        [Function(nameof(SayHelloActivity))] // ✅ Updated name reference
        public static string Run([ActivityTrigger] string name, FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger(nameof(SayHelloActivity)); // ✅ Improved logging reference
            logger.LogInformation("Saying hello to {name}.", name);
            return $"Hello {name}!";
        }
    }
}