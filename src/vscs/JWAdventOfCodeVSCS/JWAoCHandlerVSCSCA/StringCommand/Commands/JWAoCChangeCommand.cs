using JWAoCHandlerVSCSCA.Command;

namespace JWAoCHandlerVSCSCA.StringCommand.Commands;

public class JWAoCChangeCommand : JWAoCStringCommandBase
{
    public override string Name { get { return "change"; } set { } }

    public override string Description { get { return "Change the current task."; } set { } }

    public int? TaskYear { get; set; }

    public int? TaskDay { get; set; }

    public string? SubTask { get; set; }

    public string? Type { get; set; }
}