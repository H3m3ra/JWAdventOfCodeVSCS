using JWAdventOfCodeHandlerLibrary.Command;
using JWAoCHandlerVSCSCA.StringCommand.Commands;

namespace JWAoCHandlerVSCSCA.StringCommand.Factories;

public class JWAoCSetCommandFactory : JWAoCStringCommandFactoryBase
{
    // get-methods
    protected override IJWAoCStringCommand? CreateCommandFromString(string source, string trimmedSource, string simpleSource, string origin)
    {
        source = source.TrimStart();
        int nextIndex;
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
            Source = origin
        };
    }
}