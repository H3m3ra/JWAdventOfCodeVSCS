using JWAdventOfCodeHandlingLibrary.Services;

namespace JWAoCHandlerVSCSCA.Command.Commands.StringCommands;

public class JWAoCCallCommand : JWAoCStringCommandBase
{
    public virtual bool Testing { get; set; }

    public virtual string ProgramName { get; set; } = null!;

    public virtual Dictionary<string, string> ProgramArgs { get; set; } = new Dictionary<string, string>();

    public override string[] Args
    {
        get
        {
            var args = new string[1 + 2 * ProgramArgs.Count];
            args[0] = ProgramName;
            var index = 1;
            foreach (var entry in ProgramArgs)
            {
                args[index++] = entry.Key;
                args[index++] = entry.Value;
            }
            return args;
        }
        set { }
    }

    public override string Description { get { return "Call an extern program."; } set { } }

    public JWAoCCallCommand()
    {

    }

    // get-methods
    public string[] GetSolveCallArgs(string version, int year, int day, string sub, string inputFilePath, bool check)
    {
        return new string[]
            {
                "http",
                "GET",
                $"/{version}/{year}/{(day < 10 ? "0" : "")}{day}/{JWAoCHTTPService.ToURIStringFromString(sub)}?"+
                (check ? "check=true&" : "")+
                $"input={JWAoCHTTPService.ToURIStringFromString(inputFilePath)}"+
                (ProgramArgs.Count == 0 ? "" : $"&{string.Join('&', ProgramArgs.ToArray().Select(e => JWAoCHTTPService.ToURIStringFromString(e.Key)+"="+JWAoCHTTPService.ToURIStringFromString(e.Value)))}")
            };
    }
}