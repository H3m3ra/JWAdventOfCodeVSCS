using JWAdventOfCodeHandlerLibrary.Data;
using JWAdventOfCodeHandlerLibrary.Handler;
using JWAdventOfCodeHandlerLibrary.Settings.Program;
using JWAdventOfCodeHandlingLibrary.HTTP;
using JWAoCHandlerVSCSCA.Command.Commands.StringCommands;
using System.Text.Json;

namespace JWAoCHandlerVSCSCA.Handlers.CommandHandlers;

public class JWAoCCallCommandHandler : JWAoCSpecificCommandHandler<JWAoCCallCommand>
{
    public JWAoCHandlerVSCS Handler { get; set; }

    // methods
    public override bool HandleSpecificCommand(JWAoCCallCommand command)
    {
        return Execute(command.Testing ? Handler.Settings.TestType : Handler.Settings.InputType, command);
    }

    protected bool Execute(string type, JWAoCCallCommand command)
    {
        if (Handler.LoadSettrings("  Cannot ", true) && Handler.Settings.Programs.ContainsKey(command.ProgramName))
        {
            var program = Handler.Settings.Programs[command.ProgramName];

            var sourceFilePath = Handler.GetSourceFilePaths(Handler.Settings.InputsSourcePaths, type).FirstOrDefault();
            if (string.IsNullOrEmpty(sourceFilePath)) return true;

            JWAoCProgram.GetHighestVersionOf(program.GetVersions(Handler.ProgramExecutionService));
            var version = JWAoCProgram.GetHighestVersionOf(program.GetVersions(Handler.ProgramExecutionService));
            var programVersion = Handler.ProgramExecutionService.CallProgramWithLocalHTTPGet(program, $"/{version}/version").Content?.ToString();
            programVersion = programVersion == null ? null : JsonSerializer.Deserialize<string>(programVersion);
            var programAuthor = Handler.ProgramExecutionService.CallProgramWithLocalHTTPGet(program, $"/{version}/author").Content?.ToString();
            programAuthor = programAuthor == null ? null : JsonSerializer.Deserialize<string>(programAuthor);

            IJWAoCHTTPResponse response;

            if (string.IsNullOrEmpty(version))
            {
                response = new JWAoCHTTPErrorResponse(new JWAoCHTTPProblemDetails("Not able to request without a matching version!", 404));
            }
            else
            {
                var args = command.GetSolveCallArgs(version, (int)Handler.CurrentYear, (int)Handler.CurrentDay, Handler.CurrentSub, sourceFilePath);
                Handler.IOConsoleService.PrintLineOut($"  \"{command.ProgramName}\" with \"{string.Join(" ", args)}\" starting...");

                var start = DateTime.Now;
                response = Handler.ProgramExecutionService.CallProgramWithLocalHTTP(program, args);
                var duration = DateTime.Now - start;

                Handler.IOConsoleService.PrintLineOut($"  ...\"{command.ProgramName}\" finished. ({duration})");
                Handler.ResultHandlerService.HandleResult(
                    new JWAoCResult()
                    {
                        Timestamp = DateTime.Now,
                        TaskYear = (int)Handler.CurrentYear,
                        TaskDay = (int)Handler.CurrentDay,
                        SubTask = Handler.CurrentSub,
                        Duration = duration,
                        ProgramName = command.ProgramName,
                        ProgramVersion = programVersion,
                        ProgramAuthor = programAuthor,
                        Program = program,
                        ProgramArgs = args,
                        Response = response
                    },
                    Handler.Settings,
                    Handler.IOConsoleService
                );
            }
            if (response.StatusCode == 200) Handler.IOConsoleService.PrintLineOut($"{response.Content.ToString()}");
            else Handler.IOConsoleService.PrintLineOut($"ERROR {response.StatusCode}: {response.StatusName}");
        }
        return true;
    }
}