using JWAdventOfCodeHandlerLibrary.Command;
using JWAoCHandlerVSCSCA.StringCommand.Commands;

namespace JWAoCHandlerVSCSCA.StringCommand.Factories;

public class JWAoCGetCommandFactory : JWAoCStringCommandFactoryBase
{
    // get-methods
    protected override IJWAoCStringCommand? CreateCommandFromString(string source, string trimmedSource, string simpleSource, string origin)
    {
        source = source.TrimStart();
        int nextIndex = (nextIndex = source.IndexOf(' ')) < 0 ? source.Length : nextIndex;
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
            Source = origin
        };
    }
}