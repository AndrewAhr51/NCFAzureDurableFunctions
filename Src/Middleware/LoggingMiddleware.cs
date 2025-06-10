using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;

namespace NCFAzureDurableFunctions.src.Middleware
{
    internal class LoggingMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(ILogger<LoggingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            var functionName = context.FunctionDefinition.Name;
            _logger.LogInformation("Executing function: {FunctionName} at {Time}", functionName, DateTime.UtcNow);

            try
            {
                await next(context); // ✅ Proceed to function execution
                _logger.LogInformation("Function {FunctionName} completed successfully.", functionName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in function {FunctionName}: {Message}", functionName, ex.Message);
                throw; // ✅ Ensure exception is propagated correctly
            }
        }
    }
}