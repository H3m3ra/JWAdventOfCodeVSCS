namespace JWAdventOfCodeHandlerLibrary.Command;

public abstract class JWAoCStringCommandBase : IJWAoCStringCommand
{
    public virtual string Name { get; protected set; }

    public virtual string[] Args { get; protected set; }

    public virtual string Source { get; protected set; }

    public virtual string Description { get; protected set; }

    public virtual string Help { get; protected set; }

    public JWAoCStringCommandBase()
    {

    }
}