using JWAdventOfCodeHandlerLibrary.Command;

namespace JWAoCHandlerVSCSCA.Command;

public abstract class JWAoCStringCommandBase : IJWAoCStringCommand
{
    public virtual string Name { get; set; } = null!;

    public virtual string[] Args { get; set; } = null!;

    public virtual string Source { get; set; } = null!;

    public virtual string Description { get; set; } = string.Empty;

    public virtual string Help { get; set; } = string.Empty;

    public JWAoCStringCommandBase()
    {

    }
}