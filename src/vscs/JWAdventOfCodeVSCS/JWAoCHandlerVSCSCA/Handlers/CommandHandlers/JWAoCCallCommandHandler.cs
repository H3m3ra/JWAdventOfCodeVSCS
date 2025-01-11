using JWAdventOfCodeHandlerLibrary.Data;
using JWAdventOfCodeHandlerLibrary.Handler;
using JWAdventOfCodeHandlerLibrary.Services;
using JWAdventOfCodeHandlerLibrary.Settings;
using JWAdventOfCodeHandlerLibrary.Settings.Check;
using JWAdventOfCodeHandlerLibrary.Settings.Program;
using JWAdventOfCodeHandlingLibrary.HTTP;
using JWAoCHandlerVSCSCA.Command.Commands.StringCommands;
using System;
using System.Text.Json;

namespace JWAoCHandlerVSCSCA.Handlers.CommandHandlers;

public class JWAoCCallCommandHandler : JWAoCSpecificCommandHandler<JWAoCCallCommand>
{
    public IJWAoCSettingsService<JWAoCVSCSSettings> SettingsService { get; protected set; }
    public IJWAoCProgramExecutionService ProgramExecutionService { get; protected set; }
    public IJWAoCIOConsoleService IOConsoleService { get; protected set; }
    public IJWAoCResultHandlerService ResultHandlerService { get; protected set; }

    public JWAoCHandlerVSCS Handler { get; set; } = null!;

    public JWAoCCallCommandHandler(
        IJWAoCSettingsService<JWAoCVSCSSettings> currentSettingsService,
        IJWAoCProgramExecutionService currentProgramExecutionService,
        IJWAoCIOConsoleService currentIOConsoleService,
        IJWAoCResultHandlerService currentResultHandlerService
    )
    {
        SettingsService = currentSettingsService;
        ProgramExecutionService = currentProgramExecutionService;
        IOConsoleService = currentIOConsoleService;
        ResultHandlerService = currentResultHandlerService;
    }

    // methods
    public override bool HandleSpecificCommand(JWAoCCallCommand command)
    {
        var settings = SettingsService.GetSettings();

        ArgumentNullException.ThrowIfNull(settings, nameof(settings));

        return Execute(command.Testing ? settings.TestType : settings.InputType, command);
    }

    protected bool Execute(string type, JWAoCCallCommand command)
    {
        if (
            Handler.CurrentYear != null &&
            Handler.LoadSettrings("  Cannot ", true) &&
            Handler.Settings != null && Handler.Settings.Programs.ContainsKey(command.ProgramName)
        )
        {
            var settings = SettingsService.GetSettings();
            ArgumentNullException.ThrowIfNull(settings, nameof(settings));

            var program = Handler.Settings.Programs[command.ProgramName];

            JWAoCProgram.GetHighestVersionOf(program.GetVersions(ProgramExecutionService));
            var version = JWAoCProgram.GetHighestVersionOf(program.GetVersions(ProgramExecutionService));
            if (string.IsNullOrEmpty(version))
            {
                PrintResponseResult( new JWAoCHTTPErrorResponse(new JWAoCHTTPProblemDetails("Not able to request without a matching version!", 404)));
                return true;
            }

            bool lazyLoadedMetaDataLoaded = false;
            string? programVersion = null;
            string? programAuthor = null;
            void LazyLoadMetaData()
            {
                if (!lazyLoadedMetaDataLoaded)
                {
                    programVersion = ProgramExecutionService.CallProgramWithLocalHTTPGet(program, $"/{version}/version").Content?.ToString();
                    if (programVersion != null)
                    {
                        try
                        {
                            programVersion = JsonSerializer.Deserialize<string>(programVersion);
                        }
                        catch { }
                    }
                    programAuthor = ProgramExecutionService.CallProgramWithLocalHTTPGet(program, $"/{version}/author").Content?.ToString();
                    if (programAuthor != null)
                    {
                        try
                        {
                            programAuthor = JsonSerializer.Deserialize<string>(programAuthor);
                        }
                        catch { }
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

                    var sourceFilePath = Handler.GetSourceFilePaths(settings.InputsSourcePaths, type).FirstOrDefault();

                    var defaultArgs = command.GetSolveCallArgs(version, (int)Handler.CurrentYear, d, s, sourceFilePath ?? string.Empty, false);
                    IOConsoleService.PrintOut($"\"{command.ProgramName}\" with \"{string.Join(" ", defaultArgs)}\" starting...");

                    if (string.IsNullOrEmpty(sourceFilePath) || !File.Exists(sourceFilePath))
                    {
                        IOConsoleService.Print($" stopped. {Environment.NewLine}");
                        PrintResponseResult(new JWAoCHTTPErrorResponse(new JWAoCHTTPProblemDetails($"File \"{sourceFilePath}\" not found!", 404)));
                    }
                    else
                    {
                        var result = ExecuteForSpecificTask(
                            version, program, (int)Handler.CurrentYear, d, s, sourceFilePath,
                            command, settings
                        );
                        if(result != null)
                        {
                            LazyLoadMetaData();
                            result.ProgramVersion = programVersion ?? result.ProgramVersion;
                            result.ProgramAuthor = programAuthor ?? result.ProgramAuthor;

                            IOConsoleService.Print($" finished. ({result.Duration}){Environment.NewLine}");

                            if (!command.Testing)
                            {
                                ResultHandlerService.HandleResult(result, settings, IOConsoleService);
                            }
                            PrintResponseResult(result.Response);
                        }
                    }
                }
            }

            Handler.CurrentDay = bufferedCurrentDay;
            Handler.CurrentSub = bufferedCurrentSub;
        }
        return true;
    }

    protected JWAoCResult? ExecuteForSpecificTask(
        string version, JWAoCProgram program, int taskYear, int taskDay, string subTask, string sourceFilePath,
        JWAoCCallCommand command, IJWAoCSettings settings
    )
    {
        JWAoCResult? checkResult = null;
        if (settings.CheckMode != JWAoCCheckMode.NONE)
        {
            checkResult = CheckSpecificTask(version, program, taskYear, taskDay, subTask, sourceFilePath, command, settings);
        }

        if (settings.CheckMode == JWAoCCheckMode.NONE || checkResult != null)
        {
            if (settings.CheckMode == JWAoCCheckMode.STRICT && checkResult != null)
            {
                checkResult.Response.Content = null;
            }

            if (checkResult != null && checkResult.Response.Content != null)
            {
                return checkResult;
            }
            else
            {
                return RequestSpecificTaskResult(version, program, taskYear, taskDay, subTask, sourceFilePath, command, settings);
            }
        }
        return null;
    }

    protected JWAoCResult? CheckSpecificTask(
        string version, JWAoCProgram program, int taskYear, int taskDay, string subTask, string sourceFilePath,
        JWAoCCallCommand command, IJWAoCSettings settings
    )
    {
        var programResponse = GetProgramResponse(version, program, taskYear, taskDay, subTask, sourceFilePath, true, command);
        var checkResponse = programResponse.Item2;

        if (checkResponse.StatusCode != 200)
        {
            IOConsoleService.Print($" stopped. {Environment.NewLine}");
            PrintResponseResult(checkResponse);
            return null;
        }

        if (settings.CheckMode == JWAoCCheckMode.STRICT)
        {
            IOConsoleService.Print($" checked...");
        }

        return new JWAoCResult()
        {
            Timestamp = DateTime.Now,
            TaskYear = taskYear,
            TaskDay = taskDay,
            SubTask = subTask,
            Duration = programResponse.Item3,
            ProgramName = command.ProgramName,
            ProgramVersion = "Unknown",
            ProgramAuthor = "Unknown",
            Program = program,
            ProgramArgs = programResponse.Item1,
            Response = checkResponse
        };
    }

    protected JWAoCResult RequestSpecificTaskResult(
        string version, JWAoCProgram program, int taskYear, int taskDay, string subTask, string sourceFilePath,
        JWAoCCallCommand command, IJWAoCSettings settings
    )
    {
        ArgumentNullException.ThrowIfNull(Handler.Settings, nameof(Handler.Settings));

        var programResponse = GetProgramResponse(version, program, taskYear, taskDay, subTask, sourceFilePath, false, command);

        return new JWAoCResult()
        {
            Timestamp = DateTime.Now,
            TaskYear = taskYear,
            TaskDay = taskDay,
            SubTask = subTask,
            Duration = programResponse.Item3,
            ProgramName = command.ProgramName,
            ProgramVersion = "Unknown",
            ProgramAuthor = "Unknown",
            Program = program,
            ProgramArgs = programResponse.Item1,
            Response = programResponse.Item2
        };
    }

    protected Tuple<string[], IJWAoCHTTPResponse, TimeSpan> GetProgramResponse(
        string version, JWAoCProgram program, int taskYear, int taskDay, string subTask, string sourceFilePath, bool check,
        JWAoCCallCommand command
    )
    {
        var args = command.GetSolveCallArgs(version, taskYear, taskDay, subTask, sourceFilePath, check);
        var start = DateTime.Now;
        var response = ProgramExecutionService.CallProgramWithLocalHTTP(program, args);
        var duration = DateTime.Now - start;
        return Tuple.Create(args, response, duration);
    }

    protected void PrintResponseResult(IJWAoCHTTPResponse response)
    {
        if (response.StatusCode == 200)
        {
            IOConsoleService.PrintLineOut($"  {response.Content}");
        }
        else
        {
            IOConsoleService.PrintLineOut($"  ERROR {response.StatusCode}: {response.StatusName}");
            if(response.Content != null && (response.Content is JWAoCHTTPProblemDetails || !string.IsNullOrWhiteSpace(response.Content.ToString())))
            {
                var data = response.Content is JWAoCHTTPProblemDetails ? ((JWAoCHTTPProblemDetails)response.Content).Message : response.Content.ToString();
                if (data != null)
                {
                    foreach (var line in data.Split("\n").Select(l => "    " + l))
                    {
                        IOConsoleService.PrintLineOut(line);
                    }
                }
            }
        }
    }
}