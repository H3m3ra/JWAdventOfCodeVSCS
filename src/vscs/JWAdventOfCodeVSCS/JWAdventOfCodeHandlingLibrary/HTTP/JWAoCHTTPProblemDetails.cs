using System.Text.Json.Serialization;

namespace JWAdventOfCodeHandlingLibrary.HTTP;

public class JWAoCHTTPProblemDetails
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public int HTTPStatus { get; set; } = 500;

    [JsonPropertyName("detail")]
    public string Message { get; set; } = null!;

    [JsonPropertyName("instance")]
    public string Instance { get; set; } = string.Empty;

    public JWAoCHTTPProblemDetails(string message, int currentHTTPStatus)
    {
        Message = message;
        HTTPStatus = currentHTTPStatus;
    }
}