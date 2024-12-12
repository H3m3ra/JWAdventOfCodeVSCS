namespace JWAdventOfCodeHandlingLibrary.HTTP;

public class JWAoCHTTPResponse : JWAoCHTTPResponseBase
{
    public JWAoCHTTPResponse(int statusCode = 400, object content = null)
    {
        StatusCode = statusCode;
        Content = content;
    }
}