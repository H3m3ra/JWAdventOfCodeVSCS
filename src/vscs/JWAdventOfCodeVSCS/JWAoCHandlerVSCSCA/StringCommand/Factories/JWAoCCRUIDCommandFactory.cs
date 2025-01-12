using JWAdventOfCodeHandlerLibrary.Command;

namespace JWAoCHandlerVSCSCA.StringCommand.Factories;

public class JWAoCCruidCommandFactory : JWAoCStringCommandFactoryBase
{
    // get-methods
    protected override IJWAoCStringCommand? CreateCommandFromString(string source, string trimmedSource, string simpleSource, string origin)
    {
        string originalSource = source;

        return null;
    }
}