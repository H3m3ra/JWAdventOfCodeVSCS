using JWAdventOfCodeHandlerLibrary.Settings.Program;

namespace JWAdventOfCodeHandlerLibrary.Settings;

public interface IJWAoCSettings
{
    public string TasksSourcePath { get; set; }

    public string InputsSourcePath { get; set; }

    public string TestsSourcePath { get; set; }

    public string ResultsTargetPath { get; set; }

    public string SpecificResultTargetPath { get; set; }

    public Dictionary<string, JWAoCProgram> Programs { get; set; }

    // init-methods
    public bool Init();
}