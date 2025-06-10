using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace NCFAzureDurableFunctions.Src.Functions.Activities // ✅ Moved to Activities folder for clarity
{
    public static class SendConfirmationActivity
    {
        [Function(nameof(SendConfirmationActivity))]
        public static async Task Run([ActivityTrigger] string donorId, FunctionContext context)
        {
            await new HttpClient().PostAsync($"https://ncfapi.com/api/emails/{donorId}", null);
        }
    }
}