using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace NCFAzureDurableFunctions.Src.Functions.Activities // ✅ Moved to Activities folder for clarity
{
    public static class CompensatingDonorActivity
    {
        [Function(nameof(CompensatingDonorActivity))]
        public static async Task Run([ActivityTrigger] string donorId, FunctionContext context)
        {
            await new HttpClient().DeleteAsync($"https://ncfapi.com/api/donors/{donorId}");
        }
    }
}