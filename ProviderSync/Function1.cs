using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Extensions.Timer;

namespace ProviderSync
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;
        private readonly HttpClient _httpClient;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
        }

        [Function("BatchFunction")]
        public async Task RunCron([TimerTrigger("%Time%")] TimerInfo timerInfo, FunctionContext ctx, HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string token = await GetAccessTokenAsync();

            await Console.Out.WriteLineAsync($"token: {token}");

            string endpoint = "<end point>";

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // GraphQL mutation body
            var itemName = "MyNewItemSB Final2wQA";
            var parentId = "{6F2E82BE-82FD-4444-92AC-141DC4E3E61D}";
            var templateId = "{6429BD7B-C674-4249-A95B-E382A2672901}"; 
            var itemTitle = "Hello, Sitecore!";
            var category = "blog Updated";
            var updateItemId = "{67A618D4-02E3-428A-931A-AD2F1725D05F}";
            var jsonBody = string.Empty;

            if (string.IsNullOrEmpty(updateItemId))
            {
                // Create new item

                jsonBody = JsonSerializer.Serialize(new
                {
                    query = @"mutation($input: CreateItemInput!) {
        createItem(input: $input) {
            item {
                itemId
                name
                path
            }
        }
    }",
                    variables = new
                    {
                        input = new
                        {
                            database = "master",
                            name = itemName,
                            templateId = templateId,
                            parent = parentId,
                            language = "en",
                            fields = new[]
                       {
                new { name = "title", value = itemTitle },
                new { name = "category", value = category }
            }
                        }
                    }
                });
            }
            else
            {
                // Update existing item

                jsonBody = JsonSerializer.Serialize(new
                {
                    query = @"mutation($input: UpdateItemInput!) {
        updateItem(input: $input) {
            item {
                name
                itemId
                fields(ownFields: true) {
                    nodes {
                        name
                        value
                    }
                }
            }
        }
    }",
                    variables = new
                    {
                        input = new
                        {
                            database = "master",
                            itemId = updateItemId,
                            language = "en",
                            version = 1,
                            fields = new[]
          {
                new { name = "title", value = itemTitle },
                new { name = "category", value = category }
            }
                        }
                    }
                });
            }

            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            HttpResponseMessage apiResponse = await _httpClient.PostAsync(endpoint, content);
            string responseString = await apiResponse.Content.ReadAsStringAsync();

            _logger.LogInformation("GraphQL Response: {0}", responseString);

            // Return response to caller
            var response = req.CreateResponse(apiResponse.IsSuccessStatusCode ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            await response.WriteStringAsync($"Status: {apiResponse.StatusCode}\nResponse: {responseString}");

        }

        private async Task<string> GetAccessTokenAsync()
        {
            using var client = new HttpClient();

            var tokenEndpoint = "https://auth.sitecorecloud.io/oauth/token";

            var form = new Dictionary<string, string>
            {
                { "audience", "https://api.sitecorecloud.io" },
                { "grant_type", "client_credentials" },
                { "client_id", "<client-id>" },      // TODO: Store in Key Vault or env var
                { "client_secret", "<client-secret>" }
            };

            var response = await client.PostAsync(tokenEndpoint, new FormUrlEncodedContent(form));
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get token: {content}");
            }

            using var doc = System.Text.Json.JsonDocument.Parse(content);
            if (doc.RootElement.TryGetProperty("access_token", out var accessToken))
            {
                return accessToken.GetString() ?? throw new Exception("Access token is null.");
            }

            throw new Exception("Access token property not found in the response.");
        }

    }
}
