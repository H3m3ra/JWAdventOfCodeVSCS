using System.Text.RegularExpressions;
using JWAoCHandlerVSCSCA.Command;

namespace JWAoCHandlerVSCSCA.StringCommand.Commands;

public class JWAoCAddCommand : JWAoCStringCommandBase
{
    public override string Name { get { return "add"; } set { } }
    public override string Description { get { return "Add a property."; } set { } }

    // static-to-methods
    public static JWAoCAddCommand ToGetCommandFromString(string source)
    {
        if (source.Trim().Length == 0) return null;
        string originalSource = source;

        var args = new Regex("\\s+").Split(source);

        return new JWAoCAddCommand()
        {
            Args = args,
            Source = originalSource
        };
    }
}