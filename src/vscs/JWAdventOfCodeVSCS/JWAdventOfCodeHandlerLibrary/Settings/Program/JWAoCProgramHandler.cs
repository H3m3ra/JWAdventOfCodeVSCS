using System.Text.Json.Serialization;

namespace JWAdventOfCodeHandlerLibrary.Settings.Program;

public class JWAoCProgramHandler
{
    [JsonPropertyName("builder")]
    public string? BuilderFilePath { get; set; }

    [JsonPropertyName("interpreter")]
    public string? InterpreterFilePath { get; set; }

    [JsonPropertyName("compiler")]
    public string? CompilerFilePath { get; set; }
}