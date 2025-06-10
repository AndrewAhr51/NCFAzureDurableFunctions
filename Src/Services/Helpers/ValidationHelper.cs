using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NCFAzureDurableFunctions.Src.Services.Helpers
{
    public static class ValidationHelper
    {
        public static bool IsValid<T>(T model, out List<ValidationResult> results)
        {
            var context = new ValidationContext(model, null, null);
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        }
    }
}