namespace JWAdventOfCodeHandlerLibrary;

public interface IJWAoCConsolePrinter
{
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