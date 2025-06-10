using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using NCFAzureDurableFunctions.Src.Functions.Orchestrators;

namespace NCFAzureDurableFunctions.Src.Functions.Triggers
{
    public static class StartNCFDonor
    {
        [Function("NCFDonorFunction_HttpStart")]
        public static async Task<HttpResponseData> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            [DurableClient] DurableTaskClient client,
            FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger(nameof(StartNCFDonor));

            try
            {
                string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
                    nameof(NCFDonorOrchestrator));

                logger.LogInformation("Started orchestration with ID = {instanceId}", instanceId);
                return client.CreateCheckStatusResponse(req, instanceId);
            }
            catch (Exception ex)
            {
                logger.LogError("Failed to start orchestration: {Message}", ex.Message);
                var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync("Failed to start orchestration.");
                return errorResponse;
            }
        }
    }
}