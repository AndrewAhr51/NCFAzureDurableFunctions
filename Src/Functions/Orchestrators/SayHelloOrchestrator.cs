using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using NCFAzureDurableFunctions.Src.Functions.Activities;

namespace NCFAzureDurableFunctions.Src.Functions.Orchestrators
{
    public static class SayHelloOrchestrator
    {
        [Function(nameof(SayHelloOrchestrator))]
        public static async Task<List<string>> RunOrchestrator([OrchestrationTrigger] TaskOrchestrationContext context)
        {
            ILogger logger = context.CreateReplaySafeLogger(nameof(SayHelloOrchestrator));
            logger.LogInformation("Executing orchestrator: {OrchestratorName}", nameof(SayHelloOrchestrator));

            var outputs = new List<string>();

            try
            {
                outputs.Add(await context.CallActivityAsync<string>(nameof(SayHelloActivity), "Tokyo"));
                outputs.Add(await context.CallActivityAsync<string>(nameof(SayHelloActivity), "Seattle"));
                outputs.Add(await context.CallActivityAsync<string>(nameof(SayHelloActivity), "London"));
            }
            catch (Exception ex)
            {
                logger.LogError("Activity execution failed: {Message}", ex.Message);
                return new List<string> { "Error in orchestrator execution." };
            }

            return outputs;
        }
    }
}