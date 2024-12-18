using JWAdventOfCodeHandlerLibrary.Command;

namespace JWAdventOfCodeHandlerLibrary.Services;

public interface IJWAoCCommandHandlerService
{
    // methods
    public bool HandleCommand(IJWAoCCommand command);

    // get-methods
    public bool CanHandle(IJWAoCCommand command);
}