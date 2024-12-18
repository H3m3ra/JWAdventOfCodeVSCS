using JWAdventOfCodeHandlerLibrary.Settings.Program;
using JWAdventOfCodeHandlingLibrary.HTTP;

namespace JWAdventOfCodeHandlerLibrary.Data;

public class JWAoCResult
{
    public DateTime Timestamp { get; set; }

    public int TaskYear {  get; set; }

    public int TaskDay { get; set; }

    public string SubTask { get; set; } = null!;

    public TimeSpan Duration { get; set; }

    public string ProgramName { get; set; } = null!;

    public string ProgramVersion { get; set; } = null!;

    public string ProgramAuthor { get; set; } = null!;

    public JWAoCProgram Program { get; set; } = null!;

    public string[] ProgramArgs { get; set; } = null!;

    public IJWAoCHTTPResponse Response { get; set; } = null!;
}