namespace JWAdventOfCodeHandlingLibrary.HTTP;

public class JWAoCHTTPResponse : JWAoCHTTPResponseBase
{
    public static readonly IJWAoCHTTPResponse RESPOND_OK = new JWAoCHTTPResponse(200);
    public static readonly IJWAoCHTTPResponse RESPOND_BAD_REQUEST = new JWAoCHTTPResponse(400);

    public JWAoCHTTPResponse(int statusCode = 400, object? content = null)
    {
        StatusCode = statusCode;
        Content = content;
    }
}