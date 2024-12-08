using JWAdventOfCodeHandlerLibrary.Command;
using JWAdventOfCodeHandlingLibrary.Services;
using System.Text.RegularExpressions;

namespace JWAoCHandlerVSCSCA.Commands.StringCommands;

public class JWAoCCallCommand : JWAoCStringCommandBase
{
    public virtual string ProgramName { get; protected set; }

    public virtual Dictionary<string, string> ProgramArgs { get; protected set; }

    public override string[] Args
    {
        get
        {
            var args = new string[1+2*ProgramArgs.Count];
            args[0] = ProgramName;
            var index = 1;
            foreach (var entry in ProgramArgs)
            {
                args[index++] = entry.Key;
                args[index++] = entry.Value;
            }
            return args;
        }
        protected set { }
    }

    public override string Description { get { return "Call an extern program."; } protected set { } }

    public JWAoCCallCommand()
    {

    }

    // static-to-methods
    public static JWAoCCallCommand ToCallCommandFromString(string source)
    {
        string originalSource = source;

        if (source.Trim().Length == 0) return null;

        int nextIndex;

        source = source.TrimStart();
        nextIndex = (nextIndex = source.IndexOf(' ')) < 0 ? source.Length : nextIndex;
        var commandName = source.Substring(0, nextIndex);

        source = source.Substring(nextIndex).Trim();

        var args = Regex.Split(source, "\\s+");
        var programArgs = new Dictionary<string, string>();
        for (int a=1;a<args.Length; a += 2)
        {
            args[a] = args[a].ToLower();
            if (args[a] == "args")
            {
                programArgs.Add("args", String.Join(' ', args.Where((arg, i) => i > a).ToArray()));
                break;
            }
            else
            {
                if (a + 1 == args.Length) return null;
                programArgs.Add(args[a], args[a+1]);
            }
        }

        return new JWAoCCallCommand()
        {
            Name = "call",
            ProgramName = args[0],
            ProgramArgs = programArgs,
            Source = originalSource
        };
    }

    // get-methods
    public string[] GetSolveCallArgs(string version, int year, int day, string sub, string inputFilePath)
    {
        return new string[]
            {
                "http",
                "GET",
                $"/{version}/{year}/{(day < 10 ? "0" : "")}{day}/{JWAoCHTTPService.ToURIStringFromString(sub)}?"+
                $"input={JWAoCHTTPService.ToURIStringFromString(inputFilePath)}"+
                (ProgramArgs.Count == 0 ? "" : $"&{String.Join('&', ProgramArgs.ToArray().Select(e => JWAoCHTTPService.ToURIStringFromString(e.Key)+"="+JWAoCHTTPService.ToURIStringFromString(e.Value)))}")
            };
    }
}