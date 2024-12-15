using JWAdventOfCodeHandlerLibrary.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JWAdventOfCodeHandlerLibrary.Settings.Program;

public class JWAoCProgram
{
    [JsonPropertyName("type")]
    public string Type { get{ return ProgramType.ToString().ToLower(); } set { ProgramType = (JWAoCProgramType)Enum.Parse(typeof(JWAoCProgramType), value.ToUpper()); } }
    [JsonIgnore]
    public JWAoCProgramType ProgramType { get; set; }

    [JsonPropertyName("src")]
    public string ProgramFilePath { get; set; }

    // static-get-methods
    public static string? GetHighestVersionOf(IList<string> versions)
    {
        return versions.Select(s => Tuple.Create(s, int.Parse(s[1..]))).OrderBy(v => v.Item2).LastOrDefault()?.Item1;
    }

    // get-methods
    public IList<string> GetVersions(IJWAoCProgramExecutionService programExecutionService)
    {
        var result = programExecutionService.CallProgramWithLocalHTTPGet(this, "/versions");
        if(result.StatusCode != 200) return new string[] { };

        try
        {
            return JsonSerializer.Deserialize<List<string>>(result.Content.ToString());
        }
        catch
        {
            return new string[] { };
        }
    }

    public bool IsAvailable()
    {
        return File.Exists(ProgramFilePath);
    }
}