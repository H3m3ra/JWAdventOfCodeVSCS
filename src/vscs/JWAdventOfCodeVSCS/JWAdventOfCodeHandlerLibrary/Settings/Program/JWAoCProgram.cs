using JWAdventOfCodeHandlerLibrary.Services;
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

    // get-methods
    public string GetVersion(IJWAoCProgramExecutionService programExecutionService)
    {
        var result = programExecutionService.CallProgramWithLocalHTTPGet(this, "/versions");
        if(result.StatusCode != 200) return null;

        try
        {
            return ((IList<string>)result.Content).Select(s => Tuple.Create(s, int.Parse(s[1..]))).OrderBy(v => v.Item2).Last().Item1;
        }
        catch
        {
            return null;
        }
    }

    public bool IsAvailable()
    {
        return File.Exists(ProgramFilePath);
    }
}