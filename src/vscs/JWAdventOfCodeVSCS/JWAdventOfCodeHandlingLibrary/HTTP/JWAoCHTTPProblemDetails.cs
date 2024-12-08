using System.Text.Json.Serialization;

namespace JWAdventOfCodeHandlingLibrary.HTTP;

public class JWAoCHTTPProblemDetails
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("status")]
    public int HTTPStatus { get; set; }

    [JsonPropertyName("detail")]
    public string Message { get; set; }

    [JsonPropertyName("instance")]
    public string Instance { get; set; }

    public JWAoCHTTPProblemDetails(string message, int currentHTTPStatus)
    {
        Message = message;
        HTTPStatus = currentHTTPStatus;
    }
}