using JWAdventOfCodeHandlerLibrary.Command;

namespace JWAdventOfCodeHandlerLibrary.Handler;

public abstract class JWAoCSpecificCommandHandler<C> : IJWAoCCommandHandler where C : class, IJWAoCCommand
{
    // methods
    public bool HandleCommand(IJWAoCCommand command)
    {
        if (!CanHandle(command)) return true;

        return HandleSpecificCommand((C)command);
    }

    public abstract bool HandleSpecificCommand(C command);

    // get-methods
    public bool CanHandle(IJWAoCCommand command)
    {
        return command != null && command.GetType() == typeof(C);
    }
}