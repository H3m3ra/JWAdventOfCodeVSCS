using System.Text.Json;

namespace JWAdventOfCodeHandlingLibrary.HTTP;

public abstract class JWAoCHTTPResponseBase : IJWAoCHTTPResponse
{
    public virtual int StatusCode { get; set; }
    
    public virtual string StatusName { get { return IJWAoCHTTPResponse.STATUS_CODE_NAMES[StatusCode]; } set { } }

    public virtual object Content { get; set; }

    // to-methods
    public override string ToString()
    {
        var result = $"HTTP/1.1 {StatusCode} {StatusName}";
        if (Content != null)
        {
            result += Environment.NewLine + Environment.NewLine;
            result += JsonSerializer.Serialize(Content, new JsonSerializerOptions { WriteIndented = true });
        }
        return result;
    }
}