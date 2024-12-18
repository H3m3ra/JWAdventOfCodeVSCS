namespace JWAdventOfCodeHandlerLibrary.Services;

public interface IJWAoCIOConsoleService
{
    public bool Silent { get; set; }
    public int IndentationLevel { get; set; }
    public string CurrentPath { get; set; }

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