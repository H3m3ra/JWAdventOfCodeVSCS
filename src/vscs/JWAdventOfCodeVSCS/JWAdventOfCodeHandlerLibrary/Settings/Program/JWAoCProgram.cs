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
    public bool IsAvailable()
    {
        return File.Exists(ProgramFilePath);
    }
}