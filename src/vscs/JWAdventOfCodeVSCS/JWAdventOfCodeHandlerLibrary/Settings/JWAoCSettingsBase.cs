using JWAdventOfCodeHandlerLibrary.Settings.Program;
using System.Text.Json.Serialization;

namespace JWAdventOfCodeHandlerLibrary.Settings;

public abstract class JWAoCSettingsBase : IJWAoCSettings
{
    [JsonPropertyName("tasks_src")]
    public virtual string TasksSourcePath { get; set; } = ".";

    [JsonPropertyName("inputs_src")]
    public virtual string InputsSourcePath { get; set; } = ".";

    [JsonPropertyName("tests_src")]
    public virtual string TestsSourcePath { get; set; } = ".";

    [JsonPropertyName("results_trg")]
    public virtual string ResultsTargetPath { get; set; } = ".";

    [JsonPropertyName("result_trg")]
    public virtual string SpecificResultTargetPath { get; set; } = "results.csv";
    [JsonIgnore]
    public virtual string ResultTargetPath { get { return Path.Join(ResultsTargetPath, SpecificResultTargetPath); } set { } }

    [JsonPropertyName("programs")]
    public Dictionary<string, JWAoCProgram> Programs { get; set; } = new Dictionary<string, JWAoCProgram>();

    public JWAoCSettingsBase()
    {

    }

    // init-methods
    public virtual bool Init()
    {
        return true;
    }
}