using JWAdventOfCodeHandlerLibrary.Command;

namespace JWAoCHandlerVSCSCA.StringCommand;

public abstract class JWAoCStringCommandFactoryBase : IJWAoCStringCommandFactory
{
    // get-methods
    public IJWAoCStringCommand? CreateCommandFromString(string? source)
    {
        if (string.IsNullOrEmpty(source)) return null;
        return CreateCommandFromString(source, source.Trim(), source.Trim().ToLower(), source);
    }

    protected abstract IJWAoCStringCommand? CreateCommandFromString(string source, string trimmedSource, string simpleSource, string origin);
}