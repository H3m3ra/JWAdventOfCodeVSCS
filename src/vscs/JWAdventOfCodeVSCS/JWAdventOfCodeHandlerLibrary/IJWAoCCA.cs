using JWAdventOfCodeHandlerLibrary.Command;
using JWAdventOfCodeHandlerLibrary.Services;

namespace JWAdventOfCodeHandlerLibrary;

public interface IJWAoCCA : IDisposable
{
    public IJWAoCIOService IOService { get; set; }
    public IJWAoCIOConsoleService IOConsoleService { get; set; }
    public IJWAoCProgramExecutionService ProgramExecutionService { get; set; }
    public IJWAoCResultHandlerService ResultHandlerService { get; set; }

    public bool Interactive { get; set; }
    public bool Silent { get; set; }

    // init-methods
    public bool Init(params string[] args);

    // execute-methods
    public bool ExecuteExternCommand(string source);

    public bool ExecuteConsoleCommand();

    public bool ExecuteCommand(IJWAoCCommand command);
}