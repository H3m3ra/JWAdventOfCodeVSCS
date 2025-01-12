using JWAdventOfCodeHandlerLibrary.Command;
using JWAoCHandlerVSCSCA.StringCommand.Commands;
using System.Text.RegularExpressions;

namespace JWAoCHandlerVSCSCA.StringCommand.Factories;

public class JWAoCCallCommandFactory : JWAoCStringCommandFactoryBase
{
    // get-methods
    protected override IJWAoCStringCommand? CreateCommandFromString(string source, string trimmedSource, string simpleSource, string origin)
    {
        source = source.TrimStart();
        int nextIndex = (nextIndex = source.IndexOf(' ')) < 0 ? source.Length : nextIndex;
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

        if (commandName.Contains("?"))
        {
            programArgs.Add("debug", "true");
        }

        return new JWAoCCallCommand()
        {
            Name = "call",
            Testing = commandName.ToLower() == "call*",
            ProgramName = args[0],
            ProgramArgs = programArgs,
            Source = origin
        };
    }
}