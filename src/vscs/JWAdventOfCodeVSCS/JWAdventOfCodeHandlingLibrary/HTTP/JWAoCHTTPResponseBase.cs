using System.Text;
using System.Text.Json;

namespace JWAdventOfCodeHandlingLibrary.HTTP;

public abstract class JWAoCHTTPResponseBase : IJWAoCHTTPResponse
{
    public virtual string Version { get; set; } = "1.1";

    public virtual int StatusCode { get; set; }
    
    public virtual string StatusName { get { return IJWAoCHTTPResponse.GetStatusNameOf(StatusCode); } set { } }

    public virtual Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

    public virtual object? Content { get; set; }

    // to-methods
    public string ToString(bool inline=false)
    {
        var builder = new StringBuilder();
        builder.Append($"HTTP/{Version} {StatusCode} {StatusName}");
        foreach(var header in Headers)
        {
            builder.Append(Environment.NewLine);
            builder.Append(header.Key);
            builder.Append(": ");
            builder.Append(header.Value);
        }
        if (Content != null)
        {
            builder.Append(Environment.NewLine);
            builder.Append(Environment.NewLine);
            builder.Append(JsonSerializer.Serialize(Content, new JsonSerializerOptions { WriteIndented = !inline }));
        }
        return (inline ? builder.ToString().Replace(Environment.NewLine, "\\n") : builder.ToString());
    }
}