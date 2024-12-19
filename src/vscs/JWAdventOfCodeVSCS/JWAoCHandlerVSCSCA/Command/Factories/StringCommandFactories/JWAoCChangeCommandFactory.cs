using JWAdventOfCodeHandlerLibrary.Command;
using JWAoCHandlerVSCSCA.Command.Commands.StringCommands;
using System.Text.RegularExpressions;

namespace JWAoCHandlerVSCSCA.Command.Factories.StringCommandFactories;

public class JWAoCChangeCommandFactory : IJWAoCStringCommandFactory
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

        return new JWAoCChangeCommand()
        {
            Name = "call",
            TaskYear = int.Parse(args[0]),
            TaskDay = int.Parse(args[1]),
            SubTask = args[2],
            Type = "input"
        };
    }
}