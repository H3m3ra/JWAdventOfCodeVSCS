using JWAdventOfCodeHandlerLibrary.Command;

namespace JWAdventOfCodeHandlerLibrary;

public abstract class JWAoCCABase : IJWAoCCA
{
    public bool Interactive { get; set; } = true;

    public bool Silent { get; set; } = false;

    // init-methods
    public abstract bool Init(params string[] args);

    public abstract void Dispose();

    // execute-methods
    public bool ExecuteExternCommand(string source)
    {
        PrintLineIn(source);
        return ExecuteCommand(source);
    }

    public bool ExecuteConsoleCommand()
    {
        PrintPrefixIn();
        return ExecuteCommand(Console.ReadLine());
    }

    protected abstract bool ExecuteCommand(string source);

    public abstract bool ExecuteCommand(IJWAoCCommand command);

    // print-methods
    protected void PrintLinesIn(params string[] lines)
    {
        if (!Silent)
        {
            foreach (var line in lines)
            {
                PrintPrefixIn();
                Console.WriteLine(line);
            }
        }
    }

    protected void PrintLinesOut(params string[] lines)
    {
        if (!Silent)
        {
            foreach (var line in lines)
            {
                PrintPrefixOut();
                Console.WriteLine(line);
            }
        }
    }

    protected void PrintLineIn(params string[] parts)
    {
        if (!Silent)
        {
            PrintPrefixIn();
            Print(parts);
            Console.WriteLine();
        }
    }

    protected void PrintLineOut(params string[] parts)
    {
        if (!Silent)
        {
            PrintPrefixOut();
            Print(parts);
            Console.WriteLine();
        }
    }

    protected abstract void PrintPrefixIn();

    protected abstract void PrintPrefixOut();

    protected void Print(params string[] parts)
    {
        if (!Silent)
        {
            foreach (var part in parts)
            {
                Console.Write(part);
            }
        }
    }

    // get-methods
    protected IList<string> GetLinesIn(int emptyLines)
    {
        IList<string> lines = new List<string>();

        PrintPrefixIn();
        string line = null;
        while ((line = Console.ReadLine()).Length > 0 || lines.Count < 1 || lines.Last().Length > 0)
        {
            lines.Add(line);
            PrintPrefixIn();
        }

        lines.RemoveAt(lines.Count - 1);

        return lines;
    }
}