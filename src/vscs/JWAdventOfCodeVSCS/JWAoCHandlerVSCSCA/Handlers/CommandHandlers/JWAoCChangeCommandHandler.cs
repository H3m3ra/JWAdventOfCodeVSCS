using JWAdventOfCodeHandlerLibrary.Handler;
using JWAoCHandlerVSCSCA.Command.Commands.StringCommands;

namespace JWAoCHandlerVSCSCA.Handlers.CommandHandlers;
public class JWAoCChangeCommandHandler : JWAoCSpecificCommandHandler<JWAoCChangeCommand>
{
    public JWAoCHandlerVSCS Handler { get; set; }

    // methods
    public override bool HandleSpecificCommand(JWAoCChangeCommand command)
    {
        Handler.CurrentYear = command.TaskYear;
        Handler.CurrentDay = command.TaskDay;
        Handler.CurrentSub = command.SubTask;
        return true;
    }
}