using JWAdventOfCodeHandlerLibrary.Data;
using JWAdventOfCodeHandlerLibrary.Handler;
using JWAdventOfCodeHandlerLibrary.Services;
using JWAdventOfCodeHandlerLibrary.Settings;
using JWAdventOfCodeHandlerLibrary.Settings.Program;
using JWAdventOfCodeHandlingLibrary.HTTP;
using JWAoCHandlerVSCSCA.Command.Commands.StringCommands;
using System;
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

            JWAoCProgram.GetHighestVersionOf(program.GetVersions(Handler.ProgramExecutionService));
            var version = JWAoCProgram.GetHighestVersionOf(program.GetVersions(Handler.ProgramExecutionService));
            var programVersion = Handler.ProgramExecutionService.CallProgramWithLocalHTTPGet(program, $"/{version}/version").Content?.ToString();
            programVersion = programVersion == null ? null : JsonSerializer.Deserialize<string>(programVersion);
            var programAuthor = Handler.ProgramExecutionService.CallProgramWithLocalHTTPGet(program, $"/{version}/author").Content?.ToString();
            programAuthor = programAuthor == null ? null : JsonSerializer.Deserialize<string>(programAuthor);

            if (string.IsNullOrEmpty(version))
            {
                PrintResponseResult(
                    new JWAoCHTTPErrorResponse(new JWAoCHTTPProblemDetails("Not able to request without a matching version!", 404)),
                    Handler.IOConsoleService
                );
            }
            else
            {
                var taskDays = Handler.CurrentDay == null ? new int[31].Select((v, i) => i+1).ToArray() : new int[] { (int)Handler.CurrentDay };
                var taskSubs = Handler.CurrentSub == null ? new string[] { "a", "b" } : new string[] { Handler.CurrentSub };
                foreach (var d in taskDays)
                {
                    foreach (var s in taskSubs)
                    {
                        RequestResult(
                            version, program, programVersion, programAuthor,
                            (int)Handler.CurrentYear, d, s, type,
                            command,
                            Handler.Settings,
                            Handler.ProgramExecutionService,
                            Handler.IOConsoleService,
                            Handler.ResultHandlerService
                        );
                    }
                }
            }
        }
        return true;
    }

    protected void RequestResult(
        string version, JWAoCProgram program, string programVersion, string programAuthor,
        int taskYear, int taskDay, string subTask, string type,
        JWAoCCallCommand command,
        IJWAoCSettings settings,
        IJWAoCProgramExecutionService currentProgramExecutionService,
        IJWAoCIOConsoleService currentIOConsoleService,
        IJWAoCResultHandlerService currentResultHandlerService
    )
    {
        Handler.CurrentYear = taskYear;
        Handler.CurrentDay = taskDay;
        Handler.CurrentSub = subTask;
        var sourceFilePath = Handler.GetSourceFilePaths(Handler.Settings.InputsSourcePaths, type).FirstOrDefault();
        if (string.IsNullOrEmpty(sourceFilePath) || !File.Exists(sourceFilePath))
        {
            PrintResponseResult(new JWAoCHTTPErrorResponse(new JWAoCHTTPProblemDetails($"File \"{sourceFilePath}\" not found!", 404)), currentIOConsoleService);
            return;
        }

        var args = command.GetSolveCallArgs(version, taskYear, taskDay, subTask, sourceFilePath);
        currentIOConsoleService.PrintOut($"\"{command.ProgramName}\" with \"{string.Join(" ", args)}\" starting...");

        var start = DateTime.Now;
        var response = currentProgramExecutionService.CallProgramWithLocalHTTP(program, args);
        var duration = DateTime.Now - start;

        currentIOConsoleService.Print($" finished. ({duration}){Environment.NewLine}");
        currentResultHandlerService.HandleResult(
            new JWAoCResult()
            {
                Timestamp = DateTime.Now,
                TaskYear = taskYear,
                TaskDay = taskDay,
                SubTask = subTask,
                Duration = duration,
                ProgramName = command.ProgramName,
                ProgramVersion = programVersion,
                ProgramAuthor = programAuthor,
                Program = program,
                ProgramArgs = args,
                Response = response
            },
            settings,
            currentIOConsoleService
        );
        PrintResponseResult(response, currentIOConsoleService);
    }

    protected void PrintResponseResult(IJWAoCHTTPResponse response, IJWAoCIOConsoleService currentIOConsoleService)
    {
        if (response.StatusCode == 200)
        {
            currentIOConsoleService.PrintLineOut($"  {response.Content.ToString()}");
        }
        else
        {
            currentIOConsoleService.PrintLineOut($"  ERROR {response.StatusCode}: {response.StatusName}");
            if(response.Content != null)
            {
                var data = response.Content is JWAoCHTTPProblemDetails ? ((JWAoCHTTPProblemDetails)response.Content).Message : response.Content.ToString();
                foreach (var line in data.Split("\n").Select(l => "    "+l))
                {
                    currentIOConsoleService.PrintLineOut(line);
                }
            }
        }
    }
}