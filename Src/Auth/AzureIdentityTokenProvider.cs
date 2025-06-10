
using Azure.Identity;
using Azure.Core;
using Microsoft.Data.SqlClient;

namespace NCFAzureDurableFunctions.Src.Auth;
public class AzureIdentityTokenProvider
{
    public string GetAccessToken()
    {
        var credential = new DefaultAzureCredential();
        var tokenRequestContext = new TokenRequestContext(new[] { "https://database.windows.net/.default" });
        var token = credential.GetToken(tokenRequestContext);
        return token.Token;
    }
}