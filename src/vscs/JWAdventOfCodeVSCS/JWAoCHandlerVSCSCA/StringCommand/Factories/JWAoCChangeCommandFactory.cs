using JWAdventOfCodeHandlerLibrary.Command;
using JWAoCHandlerVSCSCA.StringCommand.Commands;
using System.Text.RegularExpressions;

namespace JWAoCHandlerVSCSCA.StringCommand.Factories;

public class JWAoCChangeCommandFactory : JWAoCStringCommandFactoryBase
{
    // get-methods
    protected override IJWAoCStringCommand? CreateCommandFromString(string source, string trimmedSource, string simpleSource, string origin)
    {
        source = source.TrimStart();
        int nextIndex = (nextIndex = source.IndexOf(' ')) < 0 ? source.Length : nextIndex;
        var commandName = source.Substring(0, nextIndex);

        source = source.Substring(nextIndex).Trim();

        var args = Regex.Split(source, "\\s+");

        return new JWAoCChangeCommand()
        {
            Name = "call",
            TaskYear = int.Parse(args[0]),
            TaskDay = args.Length >= 2 ? int.Parse(args[1]) : null,
            SubTask = args.Length >= 3 ? args[2] : null,
            Type = "input",
            Source = origin
        };
    }
}