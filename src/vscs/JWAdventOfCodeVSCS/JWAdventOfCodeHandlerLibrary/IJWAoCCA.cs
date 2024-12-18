using JWAdventOfCodeHandlerLibrary.Command;

namespace JWAdventOfCodeHandlerLibrary;

public interface IJWAoCCA : IJWAoCConsolePrinter, IDisposable
{
    public bool Interactive { get; set; }

    public bool Silent { get; set; }

    // init-methods
    public bool Init(params string[] args);

    // execute-methods
    public bool ExecuteExternCommand(string source);

    public bool ExecuteConsoleCommand();

    public bool ExecuteCommand(IJWAoCCommand command);
}