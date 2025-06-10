using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.DurableTask;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using NCFAzureDurableFunctions.Src.Functions.Activities;
using NCFAzureDurableFunctions.Src.Middleware; // ✅ Ensure LoggingMiddleware is referenced

namespace NCFAzureDurableFunctions.Src.Functions.Orchestrators
{
    public class NCFDonorOrchestrator
    {
        private readonly ILogger<NCFDonorOrchestrator> _logger;

        public NCFDonorOrchestrator(ILogger<NCFDonorOrchestrator> logger)
        {
            _logger = logger;
        }

        [Function(nameof(NCFDonorOrchestrator))]
        public async Task<string> RunOrchestrator([OrchestrationTrigger] TaskOrchestrationContext context)
        {
            _logger.LogInformation("Starting Donor Process for Orchestration ID: {InstanceId}", context.InstanceId);

            try
            {
                var donorId = await context.CallActivityAsync<string>(nameof(CreateDonorActivity), null);
                _logger.LogInformation("Created Donor ID: {DonorId}", donorId);

                var paymentSuccess = await context.CallActivityAsync<bool>(nameof(ProcessPaymentActivity), donorId);
                _logger.LogInformation("Payment success status: {PaymentStatus} for Donor {DonorId}", paymentSuccess, donorId);

                if (!paymentSuccess)
                {
                    await context.CallActivityAsync(nameof(CompensatingDonorActivity), donorId);
                    _logger.LogWarning("Payment failed. Compensating donor process triggered for ID: {DonorId}", donorId);
                    return "Donor Process failed: Payment processing error.";
                }

                await context.CallActivityAsync(nameof(SendConfirmationActivity), donorId);
                _logger.LogInformation("Confirmation email sent to Donor ID: {DonorId}", donorId);

                return "Donor Process completed successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Donor Process failed: {Message}", ex.Message);
                return "Donor Process failed due to an unexpected error.";
            }
        }
    }
}