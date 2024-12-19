using JWAdventOfCodeHandlerLibrary.Command;
using JWAoCHandlerVSCSCA.Command.Commands.StringCommands;
using System.Text.RegularExpressions;

namespace JWAoCHandlerVSCSCA.Command.Factories.StringCommandFactories;

public class JWAoCCallCommandFactory : IJWAoCStringCommandFactory
{
    // get-methods
    public IJWAoCStringCommand CreateCommandFromString(string source)
    {
        string originalSource = source;

        if (source.Trim().Length == 0) return null;

        int nextIndex;

        source = source.TrimStart();
        nextIndex = (nextIndex = source.IndexOf(' ')) < 0 ? source.Length : nextIndex;
        var commandName = source.Substring(0, nextIndex);

        source = source.Substring(nextIndex).Trim();

        var args = Regex.Split(source, "\\s+");
        var programArgs = new Dictionary<string, string>();
        for (int a = 1; a < args.Length; a += 2)
        {
            args[a] = args[a].ToLower();
            if (args[a] == "args")
            {
                programArgs.Add("args", string.Join(' ', args.Where((arg, i) => i > a).ToArray()));
                break;
            }
            else
            {
                if (a + 1 == args.Length) return null;
                programArgs.Add(args[a], args[a + 1]);
            }
        }

        return new JWAoCCallCommand()
        {
            Name = "call",
            Testing = commandName.ToLower() == "*",
            ProgramName = args[0],
            ProgramArgs = programArgs,
            Source = originalSource
        };
    }
}