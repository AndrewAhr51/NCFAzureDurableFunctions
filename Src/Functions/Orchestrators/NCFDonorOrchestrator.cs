using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using NCFAzureDurableFunctions.Src.Functions.Activities;

namespace NCFAzureDurableFunctions.Src.Functions.Orchestrators
{
    public static class NCFDonorOrchestrator
    {
        [Function(nameof(NCFDonorOrchestrator))]
        public static async Task<string> RunOrchestrator([OrchestrationTrigger] TaskOrchestrationContext context)
        {
            var logger = context.CreateReplaySafeLogger(nameof(NCFDonorOrchestrator));
            logger.LogInformation("Starting Donor Process...");

            try
            {
                var donorId = await context.CallActivityAsync<string>(nameof(CreateDonorActivity), null);
                var paymentSuccess = await context.CallActivityAsync<bool>(nameof(ProcessPaymentActivity), donorId);

                if (!paymentSuccess)
                {
                    await context.CallActivityAsync(nameof(CompensatingDonorActivity), donorId);
                    return " Donor Process failed: Payment processing error.";
                }

                await context.CallActivityAsync(nameof(SendConfirmationActivity), donorId);
                return " Donor Process completed successfully!";
            }
            catch (Exception ex)
            {
                logger.LogError($"Saga failed: {ex.Message}");
                return " Donor Process failed due to an unexpected error.";
            }
        }
    }
}