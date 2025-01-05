using System.Text.RegularExpressions;

namespace JWAoCHandlerVSCSCA.Command.Commands.StringCommands;

public class JWAoCSimpleStringCommand : JWAoCStringCommandBase
{
    public override string Name { get; set; } = null!;

    public override string[] Args { get; set; } = null!;

    public override string Source { get; set; } = null!;

    public override string Description { get; set; } = null!;

    public override string Help { get; set; } = null!;

    public JWAoCSimpleStringCommand()
    {

    }

    // static-to-methods
    public static JWAoCSimpleStringCommand? ToSimpleStringCommandFromString(string source)
    {
        if (source.Trim().Length == 0) return null;

        var parts = Regex.Split(source.Trim(), "\\s+");
        var args = new string[parts.Length - 1];
        Array.Copy(parts, 1, args, 0, args.Length);

        return new JWAoCSimpleStringCommand()
        {
            Name = parts[0].ToLower(),
            Args = args,
            Source = source
        };
    }
}