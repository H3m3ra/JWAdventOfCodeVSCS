using JWAdventOfCodeHandlerLibrary.Services;

namespace JWAoCHandlerVSCSCA.Services;

public class JWAoCIOConsoleService : IJWAoCIOConsoleService
{
    public string ProgramName { get; set; } = null!;
    public int IndentationLevel { get; set; } = 0;

    public bool Silent { get; set; } = false;

    public string CurrentPath { get; set; } = "";

    public JWAoCIOConsoleService()
    {

    }

    // print-methods
    public void PrintLinesIn(params string[] lines)
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

    public void PrintLinesOut(params string[] lines)
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

    public void PrintLineIn(params string[] parts)
    {
        if (!Silent)
        {
            PrintPrefixIn();
            Print(parts);
            Console.WriteLine();
        }
    }

    public void PrintLineOut(params string[] parts)
    {
        if (!Silent)
        {
            PrintPrefixOut();
            Print(parts);
            Console.WriteLine();
        }
    }

    public void PrintIn(params string[] parts)
    {
        if (!Silent)
        {
            PrintPrefixIn();
            Print(parts);
        }
    }

    public void PrintOut(params string[] parts)
    {
        if (!Silent)
        {
            PrintPrefixOut();
            Print(parts);
        }
    }

    public void PrintPrefixIn()
    {
        Console.Write(ProgramName);
        Console.Write(CurrentPath);
        Console.Write("> ");
        Console.Write(new string(' ', 2 * IndentationLevel));
    }

    public void PrintPrefixOut()
    {
        Console.Write('<');
        Console.Write(ProgramName);
        Console.Write(CurrentPath);
        Console.Write(new string(' ', 2 * IndentationLevel + 1));
    }

    public void Print(params string[] parts)
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
    public IList<string> GetLinesIn()
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

    public string GetLineIn()
    {
        return Console.ReadLine();
    }
}