namespace JWAdventOfCodeHandlingLibrary.HTTP;

public interface IJWAoCHTTPResponse
{
    public static readonly Dictionary<int, string> STATUS_CODE_NAMES = new Dictionary<int, string>()
    {
        {200, "OK"},
        {400, "Bad Request"},
        {404, "Not Found"},
        {422, "Unprocessable Entity"},
        {500, "Internal Server Error"},
        {501, "Not Implemented"},
        {503, "Service Unavailable"},
    };
    public static readonly IJWAoCHTTPResponse BAD_REQUEST = new JWAoCHTTPResponse(400);

    public int StatusCode { get; set; }

    public string StatusName { get; set; }

    public object Content { get; set; }

    // static-method
    public static string GetStatusNameOf(int statusCode)
    {
        if (STATUS_CODE_NAMES.ContainsKey(statusCode))
        {
            return "Unknown";
        }
        return STATUS_CODE_NAMES[statusCode];
    }

    // to-methods
    public string ToString(bool inline = false);
}