using JWAdventOfCodeHandlerLibrary.Command;
using JWAoCHandlerVSCSCA.Commands.StringCommands;

namespace JWAoCHandlerVSCSCA.Commands.StringCommandFactories;

public class JWAoCGetCommandFactory : IJWAoCStringCommandFactory
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

        source = source.Substring(nextIndex).TrimStart();
        var propertyName = source.TrimEnd();
        if (propertyName.StartsWith("i")) propertyName = "inputs_src";
        else if (propertyName.StartsWith("p")) propertyName = "programs";
        else if (propertyName.StartsWith("results")) propertyName = "results_trg";
        else if (propertyName.StartsWith("result_")) propertyName = "result_trg";
        else if (propertyName.StartsWith("ta")) propertyName = "tasks_src";
        else if (propertyName.StartsWith("te")) propertyName = "tests_src";
        else propertyName = null;

        return new JWAoCGetCommand()
        {
            Name = "get",
            PropertyName = propertyName,
            Source = originalSource
        };
    }
}