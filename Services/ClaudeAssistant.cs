using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using LubbInteractiveCreator.Core;

namespace LubbInteractiveCreator.Services;

/// <summary>Optional Anthropic Claude client for creator help and diagnostics.</summary>
/// <remarks>
/// The API key is supplied by the caller and is never written to logs, projects,
/// telemetry, or prompts. The client is not used unless the user explicitly enables it.
/// </remarks>
public sealed class ClaudeAssistant(HttpClient httpClient, string apiKey, string model = "claude-3-5-haiku-latest") : IAiAssistant
{
    private const string Endpoint = "https://api.anthropic.com/v1/messages";

    public async Task<string> CompleteAsync(string prompt, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("Claude is not configured. Add an API key through secure settings first.");
        if (string.IsNullOrWhiteSpace(prompt))
            throw new ArgumentException("A prompt is required.", nameof(prompt));

        using var request = new HttpRequestMessage(HttpMethod.Post, Endpoint);
        request.Headers.Add("x-api-key", apiKey);
        request.Headers.Add("anthropic-version", "2023-06-01");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var payload = new
        {
            model,
            max_tokens = 1024,
            system = "You are a concise assistant inside Lubb Interactive Creator. Never claim an action occurred unless the application confirmed it. Do not request or expose secrets.",
            messages = new[] { new { role = "user", content = prompt } }
        };
        request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Claude request failed with HTTP {(int)response.StatusCode}. Check the integration settings.");

        using var document = JsonDocument.Parse(body);
        var text = document.RootElement.GetProperty("content")[0].GetProperty("text").GetString();
        return string.IsNullOrWhiteSpace(text) ? "Claude returned no text." : text;
    }
}
