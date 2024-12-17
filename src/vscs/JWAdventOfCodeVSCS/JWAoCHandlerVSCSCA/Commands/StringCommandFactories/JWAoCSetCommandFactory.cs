using JWAdventOfCodeHandlerLibrary.Command;
using JWAoCHandlerVSCSCA.Commands.StringCommands;

namespace JWAoCHandlerVSCSCA.Commands.StringCommandFactories;

public class JWAoCSetCommandFactory : IJWAoCStringCommandFactory
{
    // get-methods
    public IJWAoCStringCommand CreateCommandFromString(string source)
    {
        string originalSource = source;

        if (source.Trim().Length == 0) return null;

        int nextIndex;

        source = source.TrimStart();
        if ((nextIndex = source.IndexOf(' ')) < 0) return null;
        var commandName = source.Substring(0, nextIndex);

        source = source.Substring(nextIndex).TrimStart();
        if ((nextIndex = source.IndexOf(' ')) < 0) return null;
        var propertyName = source.Substring(0, nextIndex).ToLower();
        if (propertyName.StartsWith("i"))
        {
            propertyName = "inputs_src";
        }
        else if (propertyName.StartsWith("results"))
        {
            propertyName = "results_trg";
        }
        else if (propertyName.StartsWith("result_"))
        {
            propertyName = "result_trg";
        }
        else if (propertyName.StartsWith("ta"))
        {
            propertyName = "tasks_src";
        }
        else if (propertyName.StartsWith("te"))
        {
            propertyName = "tests_src";
        }
        else
        {
            return null;
        }

        source = source.Substring(nextIndex).TrimStart();
        var propertyValue = source.TrimEnd();

        return new JWAoCSetCommand()
        {
            Name = "set",
            PropertyName = propertyName.ToLower(),
            PropertyValue = propertyValue,
            Source = originalSource
        };
    }
}