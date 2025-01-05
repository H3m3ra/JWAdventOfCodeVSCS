namespace JWAdventOfCodeHandlerLibrary.Command;

public interface IJWAoCStringCommandFactory
{
    // get-methods
    public IJWAoCStringCommand? CreateCommandFromString(string? source);
}