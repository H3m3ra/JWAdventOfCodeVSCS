namespace JWAdventOfCodeHandlerLibrary.Command;

public interface IJWAoCStringCommand : IJWAoCCommand
{
    public string[] Args { get; }

    public string Source { get; }
}