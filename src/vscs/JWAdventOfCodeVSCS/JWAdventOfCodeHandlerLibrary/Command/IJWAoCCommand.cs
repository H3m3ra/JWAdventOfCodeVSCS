namespace JWAdventOfCodeHandlerLibrary.Command;

public interface IJWAoCCommand
{
    public string Name { get; }

    public string Description { get; }

    public string Help { get; }
}