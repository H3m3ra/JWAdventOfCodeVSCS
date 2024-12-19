using JWAdventOfCodeHandlerLibrary.Command;

namespace JWAdventOfCodeHandlerLibrary.Handler;

public interface IJWAoCCommandHandler
{
    // methods
    public bool HandleCommand(IJWAoCCommand command);

    // get-methods
    public bool CanHandle(IJWAoCCommand command);
}