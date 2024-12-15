using JWAdventOfCodeHandlerLibrary.Command;

namespace JWAdventOfCodeHandlerLibrary;

public interface IJWAoCCA : IDisposable
{
    public bool Interactive { get; set; }

    public bool Silent { get; set; }

    // init-methods
    public bool Init(params string[] args);

    // execute-methods
    public bool ExecuteExternCommand(string source);

    public bool ExecuteConsoleCommand();

    public bool ExecuteCommand(IJWAoCCommand command);

    // print-methods
    public void PrintLinesIn(params string[] lines);

    public void PrintLinesOut(params string[] lines);

    public void PrintLineIn(params string[] parts);

    public void PrintLineOut(params string[] parts);

    public void PrintPrefixIn();

    public void PrintPrefixOut();

    public void Print(params string[] parts);

    // get-methods
    public IList<string> GetLinesIn();

    public string GetLineIn();
}