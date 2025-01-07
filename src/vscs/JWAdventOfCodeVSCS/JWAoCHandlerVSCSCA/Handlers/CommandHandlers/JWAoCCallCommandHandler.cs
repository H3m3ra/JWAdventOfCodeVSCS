using JWAdventOfCodeHandlerLibrary.Data;
using JWAdventOfCodeHandlerLibrary.Handler;
using JWAdventOfCodeHandlerLibrary.Services;
using JWAdventOfCodeHandlerLibrary.Settings;
using JWAdventOfCodeHandlerLibrary.Settings.Program;
using JWAdventOfCodeHandlingLibrary.HTTP;
using JWAoCHandlerVSCSCA.Command.Commands.StringCommands;
using System.Text.Json;

namespace JWAoCHandlerVSCSCA.Handlers.CommandHandlers;

public class JWAoCCallCommandHandler : JWAoCSpecificCommandHandler<JWAoCCallCommand>
{
    public JWAoCHandlerVSCS Handler { get; set; } = null!;

    // methods
    public override bool HandleSpecificCommand(JWAoCCallCommand command)
    {
        ArgumentNullException.ThrowIfNull(Handler.Settings, nameof(Handler.Settings));

        return Execute(command.Testing ? Handler.Settings.TestType : Handler.Settings.InputType, command);
    }

    protected bool Execute(string type, JWAoCCallCommand command)
    {
        ArgumentNullException.ThrowIfNull(Handler.Settings, nameof(Handler.Settings));

        if (
            Handler.CurrentYear != null &&
            Handler.LoadSettrings("  Cannot ", true) &&
            Handler.Settings.Programs.ContainsKey(command.ProgramName)
        )
        {
            var program = Handler.Settings.Programs[command.ProgramName];

            JWAoCProgram.GetHighestVersionOf(program.GetVersions(Handler.ProgramExecutionService));
            var version = JWAoCProgram.GetHighestVersionOf(program.GetVersions(Handler.ProgramExecutionService));
            if (string.IsNullOrEmpty(version))
            {
                PrintResponseResult(
                    new JWAoCHTTPErrorResponse(new JWAoCHTTPProblemDetails("Not able to request without a matching version!", 404)),
                    Handler.IOConsoleService
                );
                return true;
            }

            bool lazyLoadedMetaDataLoaded = false;
            string? programVersion = null;
            string? programAuthor = null;

            void LazyLoadMetaData()
            {
                if (!lazyLoadedMetaDataLoaded)
                {
                    programVersion = Handler.ProgramExecutionService.CallProgramWithLocalHTTPGet(program, $"/{version}/version").Content?.ToString();
                    if (programVersion != null)
                    {
                        try
                        {
                            programVersion = JsonSerializer.Deserialize<string>(programVersion);
                        }
                        catch
                        {
                            programVersion = null;
                        }
                    }
                    programAuthor = Handler.ProgramExecutionService.CallProgramWithLocalHTTPGet(program, $"/{version}/author").Content?.ToString();
                    if (programAuthor != null)
                    {
                        try
                        {
                            programAuthor = JsonSerializer.Deserialize<string>(programAuthor);
                        }
                        catch
                        {
                            programVersion = null;
                        }
                    }
                    lazyLoadedMetaDataLoaded = true;
                }
            }

            var bufferedCurrentDay = Handler.CurrentDay;
            var bufferedCurrentSub = Handler.CurrentSub;

            var taskDays = Handler.CurrentDay == null ? new int[25].Select((v, i) => i + 1).ToArray() : new int[] { (int)Handler.CurrentDay };
            var taskSubs = Handler.CurrentSub == null ? new string[] { "a", "b" } : new string[] { Handler.CurrentSub };
            foreach (var d in taskDays)
            {
                foreach (var s in taskSubs)
                {
                    Handler.CurrentDay = d;
                    Handler.CurrentSub = s;

                    var sourceFilePath = Handler.GetSourceFilePaths(Handler.Settings.InputsSourcePaths, type).FirstOrDefault();

                    var args = command.GetSolveCallArgs(version, (int)Handler.CurrentYear, d, s, sourceFilePath ?? string.Empty, false);
                    Handler.IOConsoleService.PrintOut($"\"{command.ProgramName}\" with \"{string.Join(" ", args)}\" starting...");

                    if (CheckSpecificTask(
                        version, program,
                        (int)Handler.CurrentYear, d, s, sourceFilePath,
                        command,
                        Handler.ProgramExecutionService,
                        Handler.IOConsoleService
                    ))
                    {
                        LazyLoadMetaData();
                        RequestSpecificTaskResult(
                            version, program, programVersion, programAuthor,
                            (int)Handler.CurrentYear, d, s, sourceFilePath,
                            command,
                            Handler.Settings,
                            Handler.ProgramExecutionService,
                            Handler.IOConsoleService,
                            Handler.ResultHandlerService
                        );
                    }
                }
            }

            Handler.CurrentDay = bufferedCurrentDay;
            Handler.CurrentSub = bufferedCurrentSub;
        }
        return true;
    }

    protected bool CheckSpecificTask(
        string version, JWAoCProgram program,
        int taskYear, int taskDay, string subTask, string? sourceFilePath,
        JWAoCCallCommand command,
        IJWAoCProgramExecutionService currentProgramExecutionService,
        IJWAoCIOConsoleService currentIOConsoleService
    )
    {
        if (string.IsNullOrEmpty(sourceFilePath) || !File.Exists(sourceFilePath))
        {
            currentIOConsoleService.Print($" stopped. {Environment.NewLine}");
            PrintResponseResult(new JWAoCHTTPErrorResponse(new JWAoCHTTPProblemDetails($"File \"{sourceFilePath}\" not found!", 404)), currentIOConsoleService);
            return false;
        }
        var checkResponse = currentProgramExecutionService.CallProgramWithLocalHTTP(
            program,
            command.GetSolveCallArgs(version, taskYear, taskDay, subTask, sourceFilePath, true)
        );
        if (checkResponse.StatusCode != 200)
        {
            currentIOConsoleService.Print($" stopped. {Environment.NewLine}");
            PrintResponseResult(checkResponse, currentIOConsoleService);
            return false;
        }

        return true;
    }

    protected void RequestSpecificTaskResult(
        string version, JWAoCProgram program, string? programVersion, string? programAuthor,
        int taskYear, int taskDay, string subTask, string sourceFilePath,
        JWAoCCallCommand command,
        IJWAoCSettings settings,
        IJWAoCProgramExecutionService currentProgramExecutionService,
        IJWAoCIOConsoleService currentIOConsoleService,
        IJWAoCResultHandlerService currentResultHandlerService
    )
    {
        ArgumentNullException.ThrowIfNull(Handler.Settings, nameof(Handler.Settings));

        var args = command.GetSolveCallArgs(version, taskYear, taskDay, subTask, sourceFilePath, false);

        var start = DateTime.Now;
        var response = currentProgramExecutionService.CallProgramWithLocalHTTP(program, args);
        var duration = DateTime.Now - start;

        currentIOConsoleService.Print($" finished. ({duration}){Environment.NewLine}");

        if (!command.Testing && string.Compare(command.ProgramArgs.GetValueOrDefault("debug"), "true", true) != 0)
        {
            currentResultHandlerService.HandleResult(
                new JWAoCResult()
                {
                    Timestamp = DateTime.Now,
                    TaskYear = taskYear,
                    TaskDay = taskDay,
                    SubTask = subTask,
                    Duration = duration,
                    ProgramName = command.ProgramName,
                    ProgramVersion = programVersion ?? "Unknown",
                    ProgramAuthor = programAuthor ?? "Unknown",
                    Program = program,
                    ProgramArgs = args,
                    Response = response
                },
                settings,
                currentIOConsoleService
            );
        }
        PrintResponseResult(response, currentIOConsoleService);
    }

    protected void PrintResponseResult(IJWAoCHTTPResponse response, IJWAoCIOConsoleService currentIOConsoleService)
    {
        if (response.StatusCode == 200)
        {
            currentIOConsoleService.PrintLineOut($"  {response.Content}");
        }
        else
        {
            currentIOConsoleService.PrintLineOut($"  ERROR {response.StatusCode}: {response.StatusName}");
            if(response.Content != null && (response.Content is JWAoCHTTPProblemDetails || !string.IsNullOrWhiteSpace(response.Content.ToString())))
            {
                var data = response.Content is JWAoCHTTPProblemDetails ? ((JWAoCHTTPProblemDetails)response.Content).Message : response.Content.ToString();
                if (data != null)
                {
                    foreach (var line in data.Split("\n").Select(l => "    " + l))
                    {
                        currentIOConsoleService.PrintLineOut(line);
                    }
                }
            }
        }
    }
}