
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

string apiKey = Environment.GetEnvironmentVariable("API_KEY");

if (string.IsNullOrEmpty(apiKey))
{
    Console.Error.WriteLine("Error: The API_KEY environment variable is not set");
    Environment.Exit(1);
}

// Callback endpoint 
app.MapPost("/webhook", async (HttpContext context) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();

    Console.WriteLine($"Secret key received: {body}");
    await context.Response.WriteAsync("Key received");
});

// Main logic
app.Lifetime.ApplicationStarted.Register(async () =>
{
    await Task.Delay(5000); // Wait for ngrok to start
    await RunMainLogic();
});

// Start listening for HTTP calls
app.Run();

async Task RunMainLogic()
{
    var endpoint = new Uri($"https://gfms-sandbox-monkeyisland.azurewebsites.net/{apiKey}");
    using var client = new HttpClient();

    try
    {
        int sum = await GetMagicNumbersSumAsync(client, endpoint);
        string callbackUrl = GetCallbackUrl();
        await PostSumAsync(client, endpoint, sum, callbackUrl);
    }
    catch (Exception e)
    {
        Console.WriteLine($"An error occurred: {e.StackTrace}");
    }
}

async Task<int> GetMagicNumbersSumAsync(HttpClient client, Uri endpoint)
{
    var response = await client.GetAsync(endpoint);
    response.EnsureSuccessStatusCode();

    var result = await response.Content.ReadAsStringAsync();
    Console.WriteLine($"GET numbers: {result}");

    var magicNumbers = JsonSerializer.Deserialize<MagicNumbers>(result);

    if (magicNumbers?.magicNumbers != null)
    {
        return magicNumbers.magicNumbers.Sum();
    }

    throw new InvalidDataException("magicNumbers is null!");
}

async Task PostSumAsync(HttpClient client, Uri endpoint, int sum, string callbackUrl)
{
    var solution = new Solution { sum = sum, callbackUrl = callbackUrl };
    var json = JsonSerializer.Serialize(solution);
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    Console.WriteLine($"POST parameters sum: {solution.sum}, callbackUrl: {solution.callbackUrl}");

    var response = await client.PostAsync(endpoint, content);
    response.EnsureSuccessStatusCode();

    Console.WriteLine($"POST response status code: {(int)response.StatusCode} {response.StatusCode}");
}

string GetCallbackUrl()
{
    var ngrokUrl = Environment.GetEnvironmentVariable("NGROK_URL");
    if (string.IsNullOrEmpty(ngrokUrl))
    {
        throw new InvalidOperationException("NGROK_URL environment variable is not set");
    }
    return $"{ngrokUrl}/webhook";
}

class MagicNumbers
{
    public int[]? magicNumbers { get; set; }
}

class Solution
{
    public int sum { get; set; }
    public string? callbackUrl { get; set; }
}