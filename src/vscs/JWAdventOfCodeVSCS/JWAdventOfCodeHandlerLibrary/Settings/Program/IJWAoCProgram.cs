using JWAdventOfCodeHandlerLibrary.Services;

namespace JWAdventOfCodeHandlerLibrary.Settings.Program;

public interface IJWAoCProgram
{
    public JWAoCProgramType ProgramType { get; set; }

    public TimeSpan TimeOut { get; set; }

    public string ProgramFilePath { get; set; }

    public JWAoCProgramHandler? ProgramHandler { get; set; }

    // get-methods
    public IList<string> GetVersions(IJWAoCProgramExecutionService programExecutionService);

    public bool IsAvailable();
}