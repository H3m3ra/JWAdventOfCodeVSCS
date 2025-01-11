using JWAdventOfCodeHandlerLibrary.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JWAdventOfCodeHandlerLibrary.Settings.Program;

public class JWAoCProgram : IJWAoCProgram
{
    [JsonPropertyName("type")]
    public string Type { get{ return ProgramType.ToString().ToLower(); } set { ProgramType = (JWAoCProgramType)Enum.Parse(typeof(JWAoCProgramType), value.ToUpper()); } }
    [JsonIgnore]
    public JWAoCProgramType ProgramType { get; set; }

    [JsonPropertyName("time_out")]
    public string TimeOutText { get { return TimeOut.ToString(); } set { TimeOut = TimeSpan.Parse(value); } }
    [JsonIgnore]
    public TimeSpan TimeOut { get; set; } = new TimeSpan(0, 0, 30);

    [JsonPropertyName("src")]
    public string ProgramFilePath { get; set; } = string.Empty;

    [JsonPropertyName("handler")]
    public JWAoCProgramHandler? ProgramHandler { get; set; } = null;

    // static-get-methods
    public static string? GetHighestVersionOf(IList<string> versions)
    {
        return versions.Select(s => Tuple.Create(s, int.Parse(s[1..]))).OrderBy(v => v.Item2).LastOrDefault()?.Item1;
    }

    // get-methods
    public IList<string> GetVersions(IJWAoCProgramExecutionService programExecutionService)
    {
        var result = programExecutionService.CallProgramWithLocalHTTPGet(this, "/versions");
        if (result.StatusCode != 200 || result.Content == null) return Array.Empty<string>();

        try
        {
            var content = result.Content.ToString();
            if (content == null) return Array.Empty<string>();

            var deserialisedResult = JsonSerializer.Deserialize<List<string>>(content);
            if (deserialisedResult != null) return deserialisedResult;
        }
        catch { }
        return Array.Empty<string>();
    }

    public bool IsAvailable()
    {
        return File.Exists(ProgramFilePath);
    }
}