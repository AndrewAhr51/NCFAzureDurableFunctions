using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace NCFAzureDurableFunctions.Src.Functions.Activities // ✅ Moved to Activities folder for clarity
{
    public static class CreateDonorActivity
    {
        [Function(nameof(CreateDonorActivity))]
        public static async Task<string> Run([ActivityTrigger] object input, FunctionContext context)
        {
            var response = await new HttpClient().PostAsync("https://ncfapi.com/api/donors", null);
            return await response.Content.ReadAsStringAsync();
        }
    }
}