using System.Text;

namespace JWAdventOfCodeHandlingLibrary.Services;

public class JWAoCHTTPService
{
    public const string SPECIAL_URI_SIGNS = " !#$%&'()*+,/:;=?@[]";
    public static readonly string[] SPECIAL_URI_CODES = SPECIAL_URI_SIGNS
        .Select(sign => "%" + ASCIIEncoding.ASCII.GetBytes(sign.ToString())[0].ToString("X2"))
        .ToArray();
    public const char HTTP_SPECIAL_SIGN = '%';
    public const char HTTP_PATH_SEPARATOR = '/';
    public const char HTTP_PARAMETERS_SEPARATOR = '?';
    public const char HTTP_PARAMETER_SEPARATOR = '&';
    public const char HTTP_PARAMETER_VALUE_SEPARATOR = '=';

    // static-to-methods
    public static string ToURIStringFromString(string source)
    {
        if (String.IsNullOrEmpty(source)) return source;

        var result = "";
        foreach (var currentChar in source)
        {
            var index = SPECIAL_URI_SIGNS.IndexOf(currentChar);
            if (index < 0)
            {
                result += currentChar;
            }
            else
            {
                result += SPECIAL_URI_CODES[index];
            }
        }
        return source;
    }

    public static string ToStringFromURIString(string source)
    {
        if (String.IsNullOrEmpty(source)) return source;

        var result = "";
        for(int c=0;c<source.Length;c++)
        {
            var index = -1;
            if (source[c] == HTTP_SPECIAL_SIGN && c+2 < source.Length && (index = Array.IndexOf(SPECIAL_URI_CODES, source[c..^3])) >= 0)
            {
                result += SPECIAL_URI_SIGNS[index];
            }
            else
            {
                result += source[c];
            }
        }
        return source;
    }

    // static-get-methods
    public static string[] GetRouteFromLocalURIString(string source)
    {
        source = (source.Contains(HTTP_PARAMETERS_SEPARATOR) ? source.Substring(0, source.IndexOf(HTTP_PARAMETERS_SEPARATOR)) : source);
        source = (source.StartsWith(HTTP_PATH_SEPARATOR) ? source.Substring(1) : source);
        return source.Split(HTTP_PATH_SEPARATOR).Select(p => ToStringFromURIString(p)).ToArray();
    }

    public static Dictionary<string, string> GetParametersFromLocalURIString(string source)
    {
        var parameters = new Dictionary<string, string>();

        var currentSource = (source.Contains(HTTP_PARAMETERS_SEPARATOR) ? source.Substring(source.IndexOf(HTTP_PARAMETERS_SEPARATOR) + 1) : null);
        if (string.IsNullOrWhiteSpace(currentSource)) return parameters;

        foreach (var parameterSource in currentSource.Split(HTTP_PARAMETER_SEPARATOR))
        {
            var paramParts = parameterSource.Split(HTTP_PARAMETER_VALUE_SEPARATOR);
            parameters.Add(ToStringFromURIString(paramParts[0]), ToStringFromURIString(paramParts[1]));
        }

        return parameters;
    }
}