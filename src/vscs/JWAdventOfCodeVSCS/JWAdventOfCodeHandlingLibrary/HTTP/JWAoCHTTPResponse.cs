using System.Text.Json;

namespace JWAdventOfCodeHandlingLibrary.HTTP;

public class JWAoCHTTPResponse
{
    public static readonly Dictionary<int, string> STATUS_CODE_NAMES = new Dictionary<int, string>()
    {
        {200, "OK"},
        {400, "Bad Request"},
        {404, "Not Found"},
        {500, "Internal Server Error"},
        {501, "Not Implemented"},
        {503, "Service Unavailable"},
    };
    public static readonly JWAoCHTTPResponse BAD_REQUEST = new JWAoCHTTPResponse(400);

    public int StatusCode { get; set; }
    
    public string StatusName { get { return STATUS_CODE_NAMES[StatusCode]; } set { } }

    public object Content { get; set; }

    public JWAoCHTTPResponse(int statusCode, object content=null)
    {
        StatusCode = statusCode;
        Content = content;
    }

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