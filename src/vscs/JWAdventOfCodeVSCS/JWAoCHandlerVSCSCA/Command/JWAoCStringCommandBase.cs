using JWAdventOfCodeHandlerLibrary.Command;

namespace JWAoCHandlerVSCSCA.Command;

public abstract class JWAoCStringCommandBase : IJWAoCStringCommand
{
    public virtual string Name { get; set; }

    public virtual string[] Args { get; set; }

    public virtual string Source { get; set; }

    public virtual string Description { get; set; }

    public virtual string Help { get; set; }

    public JWAoCStringCommandBase()
    {

    }
}