using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace NCFAzureDurableFunctions.Src.Services.Helpers
{
    public class AuthenticationMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _key;

        public AuthenticationMiddleware(IConfiguration config)
        {
            _issuer = config["Jwt:Issuer"];
            _audience = config["Jwt:Audience"];
            _key = config["Jwt:Key"];
        }

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            var httpReq = await context.GetHttpRequestDataAsync();
            if (httpReq == null || !httpReq.Headers.TryGetValues("Authorization", out var authHeaders))
            {
                await SetHttpResponse(context, HttpStatusCode.Unauthorized, "Missing Authorization header");
                return;
            }

            var bearerToken = authHeaders.FirstOrDefault()?.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(bearerToken))
            {
                await SetHttpResponse(context, HttpStatusCode.Unauthorized, "Invalid Authorization header");
                return;
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParams = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _issuer,
                    ValidAudience = _audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key))
                };

                tokenHandler.ValidateToken(bearerToken, validationParams, out _);
                await next(context); // Proceed to function
            }
            catch (Exception ex)
            {
                await SetHttpResponse(context, HttpStatusCode.Unauthorized, $"Unauthorized: {ex.Message}");
            }
        }

        private async Task SetHttpResponse(FunctionContext context, HttpStatusCode statusCode, string message)
        {
            var response = context.GetHttpResponseData();
            if (response != null)
            {
                response.StatusCode = statusCode;
                await response.WriteStringAsync(message);
            }
        }
    }
}