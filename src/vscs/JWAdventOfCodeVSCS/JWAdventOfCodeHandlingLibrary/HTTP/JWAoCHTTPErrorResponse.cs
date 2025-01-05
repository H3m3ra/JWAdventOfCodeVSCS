namespace JWAdventOfCodeHandlingLibrary.HTTP;

public class JWAoCHTTPErrorResponse : JWAoCHTTPResponseBase
{
    public override int StatusCode
    {
        get
        {
            return statusCode;
        }
        set
        {
            statusCode = value;
            if (Content != null)
            {
                ((JWAoCHTTPProblemDetails)Content).HTTPStatus = statusCode;
            }
        }
    }
    protected int statusCode;

    public JWAoCHTTPErrorResponse(JWAoCHTTPProblemDetails problemDetails)
    {
        statusCode = problemDetails.HTTPStatus;
        Content = problemDetails;
    }
}