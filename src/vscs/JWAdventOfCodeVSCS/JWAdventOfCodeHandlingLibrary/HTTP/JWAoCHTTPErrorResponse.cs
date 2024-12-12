namespace JWAdventOfCodeHandlingLibrary.HTTP;

public class JWAoCHTTPErrorResponse : JWAoCHTTPResponseBase
{
    public override int StatusCode
    {
        get { return statusCode; }
        set { statusCode = statusCode; ((JWAoCHTTPProblemDetails)Content).HTTPStatus = statusCode; }
    }
    protected int statusCode;

    public JWAoCHTTPErrorResponse(JWAoCHTTPProblemDetails problemDetails)
    {
        statusCode = problemDetails.HTTPStatus;
        Content = problemDetails;
    }
}