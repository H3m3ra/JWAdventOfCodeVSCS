using JWAdventOfCodeHandlingLibrary.HTTP;
using JWAdventOfCodeHandlingLibrary.Services;
using System.Text.RegularExpressions;

namespace JWAdventOfCodeProgramLibrary;

public abstract class JWAoCProgramCABase : IJWAoCProgramCA
{
    public virtual string[] HTTPAPIVersions { get; protected set; } = null!;

    public virtual string[] ProgramHelps { get; protected set; } = null!;

    public virtual string[] ProgramAuthors { get; protected set; } = null!;

    public virtual string[] ProgramVersions { get; protected set; } = null!;

    public virtual bool Debug { get; protected set; }

    // methods
    public void ConsoleResponseToLocalHTTPGetRequest(string requestURIString)
    {
        string[] route = JWAoCHTTPService.GetRouteFromLocalURIString(requestURIString);
        Dictionary<string, string> parameters = JWAoCHTTPService.GetParametersFromLocalURIString(requestURIString);

        ConsoleResponseToLocalHTTPGetRequest(route, parameters);
    }

    protected void ConsoleResponseToLocalHTTPGetRequest(string[] route, Dictionary<string, string> parameters)
    {
        Debug = string.Compare(parameters.GetValueOrDefault("debug"), "true", true) == 0;

        var REGEX_NUMBER = new Regex("^\\d+$");

        if (route.Length == 1 && route[0] == "versions")
        {
            Print(new JWAoCHTTPResponse(200, HTTPAPIVersions));
        }
        else if (route.Length == 1 && HTTPAPIVersions.Contains(route[0]))
        {
            Print(new JWAoCHTTPResponse(200, ProgramHelps[Array.IndexOf(HTTPAPIVersions, route[0])]));
        }
        else if (route.Length == 2 && HTTPAPIVersions.Contains(route[0]) && route[1] == "author")
        {
            Print(new JWAoCHTTPResponse(200, ProgramAuthors[Array.IndexOf(HTTPAPIVersions, route[0])]));
        }
        else if (route.Length == 2 && HTTPAPIVersions.Contains(route[0]) && route[1] == "version")
        {
            Print(new JWAoCHTTPResponse(200, ProgramVersions[Array.IndexOf(HTTPAPIVersions, route[0])]));
        }
        else if (
            route.Length == 4 && HTTPAPIVersions.Contains(route[0]) && REGEX_NUMBER.Match(route[1]).Success && REGEX_NUMBER.Match(route[2]).Success &&
            parameters.ContainsKey("input")
        )
        {
            ConsoleResponseToLocalHTTPSolveRequest(route[0], int.Parse(route[1]), int.Parse(route[2]), route[3], parameters);
        }
        else
        {
            Print(IJWAoCHTTPResponse.BAD_REQUEST);
        }
    }

    protected abstract void ConsoleResponseToLocalHTTPSolveRequest(string version, int taskYear, int taskDay, string subTask, Dictionary<string, string> parameters);

    // print-methods
    protected void Print(IJWAoCHTTPResponse response)
    {
        Console.WriteLine(response.ToString());
    }
}