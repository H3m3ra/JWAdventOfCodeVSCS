using System.Text.RegularExpressions;

namespace JWAoCHandlerVSCSCA.Command.Commands.StringCommands;

public class JWAoCRemoveCommand : JWAoCStringCommandBase
{
    public override string Name { get { return "rem"; } set { } }
    public override string Description { get { return "Remove a property."; } set { } }

    public JWAoCRemoveCommand()
    {

    }

    // static-to-methods
    public static JWAoCRemoveCommand ToGetCommandFromString(string source)
    {
        if (source.Trim().Length == 0) return null;
        string originalSource = source;

        var args = new Regex("\\s+").Split(source);

        return new JWAoCRemoveCommand()
        {
            Args = args,
            Source = originalSource
        };
    }

}