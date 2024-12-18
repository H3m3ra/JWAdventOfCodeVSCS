using JWAdventOfCodeHandlerLibrary.Data;
using JWAdventOfCodeHandlerLibrary.Settings;
using JWAdventOfCodeHandlerLibrary.Settings.Program;
using JWAdventOfCodeHandlingLibrary.HTTP;
using JWAdventOfCodeHandlingLibrary.Services;

namespace JWAoCHandlerVSCSCA.Command.Commands.StringCommands;

public class JWAoCCallCommand : JWAoCStringCommandBase
{
    public virtual string ProgramName { get; set; }

    public virtual Dictionary<string, string> ProgramArgs { get; set; }

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

    // methods
    public bool Execute(IJWAoCSettings settings, JWAoCHandlerVSCS handler)
    {
        if (handler.LoadSettrings("  Cannot ", true) && settings.Programs.ContainsKey(ProgramName))
        {
            var program = settings.Programs[ProgramName];

            var inputFilePath = handler.GetSourceFilePaths(new string[] { settings.InputsSourcePath }, JWAoCHandlerVSCS.INPUT_REGEX).FirstOrDefault();
            if (string.IsNullOrEmpty(inputFilePath))
            {
                inputFilePath = handler.GetSourceFilePaths(new string[] { settings.InputsSourcePath }, JWAoCHandlerVSCS.INPUT_REGEX).FirstOrDefault();
            }

            JWAoCProgram.GetHighestVersionOf(program.GetVersions(handler.ProgramExecutionService));
            var start = DateTime.Now;
            var version = JWAoCProgram.GetHighestVersionOf(program.GetVersions(handler.ProgramExecutionService));
            var referenceDuration = DateTime.Now - start;
            IJWAoCHTTPResponse response;

            if (string.IsNullOrEmpty(version))
            {
                response = new JWAoCHTTPErrorResponse(new JWAoCHTTPProblemDetails("Not able to request without a matching version!", 404));
            }
            else
            {
                var args = GetSolveCallArgs(version, (int)handler.CurrentYear, (int)handler.CurrentDay, handler.CurrentSub, inputFilePath);
                handler.IOConsoleService.PrintLineOut($"  \"{ProgramName}\" with \"{string.Join(" ", args)}\" starting...");

                start = DateTime.Now;
                response = handler.ProgramExecutionService.CallProgramWithLocalHTTP(program, args);
                var duration = DateTime.Now - start - referenceDuration;

                handler.IOConsoleService.PrintLineOut($"  ...\"{ProgramName}\" finished. ({duration})");
                handler.ResultHandlerService.HandleResult(
                    new JWAoCResult()
                    {
                        Timestamp = DateTime.Now,
                        TaskYear = (int)handler.CurrentYear,
                        TaskDay = (int)handler.CurrentDay,
                        SubTask = handler.CurrentSub,
                        Duration = duration,
                        ProgramName = ProgramName,
                        Program = program,
                        ProgramArgs = args,
                        Response = response
                    },
                    settings,
                    handler
                );
            }
            if (response.StatusCode == 200) handler.IOConsoleService.PrintLineOut($"{response.Content.ToString()}");
            else handler.IOConsoleService.PrintLineOut($"ERROR {response.StatusCode}: {response.StatusName}");
        }
        return true;
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
                (ProgramArgs.Count == 0 ? "" : $"&{string.Join('&', ProgramArgs.ToArray().Select(e => JWAoCHTTPService.ToURIStringFromString(e.Key)+"="+JWAoCHTTPService.ToURIStringFromString(e.Value)))}")
            };
    }
}