using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace NCFAzureDurableFunctions.Src.Functions.Activities // ✅ Moved to Activities folder for clarity
{
    public static class ProcessPaymentActivity
    {
        [Function(nameof(ProcessPaymentActivity))]
        public static async Task<bool> Run([ActivityTrigger] string donorId, FunctionContext context)
        {
            var response = await new HttpClient().PostAsync($"https://ncfapi.com/api/payments/{donorId}", null);
            return response.IsSuccessStatusCode;
        }
    }
}