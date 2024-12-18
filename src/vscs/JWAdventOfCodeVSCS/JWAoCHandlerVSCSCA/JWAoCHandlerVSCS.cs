﻿using JWAdventOfCodeHandlerLibrary;
using JWAdventOfCodeHandlerLibrary.Command;
using JWAdventOfCodeHandlerLibrary.Services;
using JWAdventOfCodeHandlerLibrary.Settings;
using JWAdventOfCodeHandlingLibrary.HTTP;
using JWAoCHandlerVSCSCA.Command.Commands.StringCommands;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace JWAoCHandlerVSCSCA;

public class JWAoCHandlerVSCS : JWAoCHandlerCABase<JWAoCVSCSSettings>
{
    public const string PROGRAM_NAME_FULL = "JWAdventOfCodeVSCSCA";
    public const string PROGRAM_VERSION_FULL = "1.0.0.20241215221000";
    public const string PROGRAM_NAME = "JWAoCVSCS";
    public const string PROGRAM_VERSION = "v1.0";
    public const string PROGRAM_NAME_SHORT = "AoC";

    public static readonly Regex TASK_REGEX = new Regex("task", RegexOptions.IgnoreCase);
    public static readonly Regex INPUT_REGEX = new Regex("input", RegexOptions.IgnoreCase);
    public static readonly Regex TEST_REGEX = new Regex("test", RegexOptions.IgnoreCase);

    public const string TASK_SUFFIX = "_task.txt";
    public const string INPUT_SUFFIX = "_input.txt";
    public const string TEST_SUFFIX = "_test.txt";

    public IDictionary<string, IJWAoCStringCommandFactory> CommandFactories { get; set; } = new Dictionary<string, IJWAoCStringCommandFactory>();
    public IList<IJWAoCCommandHandlerService> CommandHandlers { get; set; } = new List<IJWAoCCommandHandlerService>();
    public IJWAoCIOService IOService { get; set; } = null!;
    public IJWAoCProgramExecutionService ProgramExecutionService { get; set; } = null!;
    public IJWAoCResultHandlerService ResultHandlerService { get; set; } = null!;

    public int? CurrentYear { get; set; } = null;
    public int? CurrentDay { get; set; } = null;
    public string? CurrentSub { get; set; } = null;

    protected int printLevel = 0;

    public JWAoCHandlerVSCS()
    {
        SettingsSerializer = new JWAoCSettingsSerializer<JWAoCVSCSSettings>(
            "config.json",
            $"{PROGRAM_NAME_FULL}_{PROGRAM_VERSION_FULL}"
        );
    }

    // init-methods
    public override bool Init(params string[] args)
    {
        var options = args.Length > 0 ? args[0] : "";

        Silent = options.Contains("-s");
        Interactive = options.Contains("-i");

        Print($"{PROGRAM_NAME} {PROGRAM_VERSION} starting...");
        if (!LoadSettrings(" cannot ")) return false;
        Print($"{Environment.NewLine}");

        if (options.Contains("-f") && args.Length >= 2)
        {
            try
            {
                bool execute;
                if (Interactive)
                {
                    PrintPrefixIn();
                    Print($"Execute file from path \"{args[1]}\"? (y/n) ");
                    execute = GetLineIn().Trim().ToLower().StartsWith("y");
                }
                else
                {
                    PrintLineOut($"Execute file from path \"{args[1]}\".");
                    execute = true;
                }

                if (execute)
                {
                    printLevel = 1;
                    foreach (var line in File.ReadLines(args[1]))
                    {
                        ExecuteExternCommand(line);
                    }
                    printLevel = 0;
                }
            }
            catch(Exception ex)
            {
                printLevel = 0;
                PrintLineOut($"Cannot execute file from path \"{args[1]}\"!");
            }
        }
        return true;
    }

    public override void Dispose()
    {
        Print($"...{PROGRAM_NAME} finished.{Environment.NewLine}");
    }

    // methods
    protected override bool ExecuteCommand(string source)
    {
        if (string.IsNullOrEmpty(source)) return true;

        var trimmedSource = source.Trim();
        var simpleSource = trimmedSource.ToLower();

        foreach (var entry in CommandFactories)
        {
            if (simpleSource.StartsWith(entry.Key))
            {
                return ExecuteCommand(entry.Value.CreateCommandFromString(source));
            }
        }

        return true;
    }

    public override bool ExecuteCommand(IJWAoCCommand command)
    {
        var handlers = CommandHandlers.Where(h => h.CanHandle(command)).ToList();

        if(handlers.Count == 0)
        {
            PrintLineOut($"  Cannot handle command \"{command.GetType().Name}\"!");
            return true;
        }

        foreach (var handler in handlers)
        {
            if (!handler.HandleCommand(command))
            {
                return false;
            }
        }
        return true;
    }

    public void Help()
    {
        PrintLinesOut(
            "**** JWAoCVSCS-Help *****************************************",
            "  ?              Show help.                                  ",
            "  call    n      Call a program.                             ",
            "  change  [v]    Change back or to a year/task/specific task.",
            "  create  [v]    Change back or to a year/task/specific task.",
            "  help           Show help.                                  ",
            "  get     [c]    Get all or a specific setting.              ",
            "  quit           Quit program.                               ",
            "  set     c v    Set a specific setting.                     ",
            "  show           ",
            "*************************************************************"
        );
    }

    // load-methods
    public bool LoadSettrings(string exceptionPrefixText, bool printPrefix=false)
    {
        try
        {
            Settings = SettingsSerializer.LoadSettings();
        }
        catch (Exception ex)
        {
            if (printPrefix) PrintPrefixOut();
            Print(
                exceptionPrefixText,
                $"load settings \"{SettingsSerializer.ConfigFilePath}\" caused by ",
                (ex is IOException ? "IO related" : (ex is JsonException ? "JSON related" : "unknown")),
                $" issues!{Environment.NewLine}"
            );
            return false;
        }
        if (!Settings.Init())
        {
            if (printPrefix) PrintPrefixOut();
            Print($"{exceptionPrefixText}initalize settings!{Environment.NewLine}");
            return false;
        }
        return true;
    }

    // store-methods
    public bool StoreSettings(string exceptionPrefixText, bool printPrefix = false)
    {
        try
        {
            SettingsSerializer.StoreSettings(Settings);
        }
        catch (Exception ex)
        {
            if (printPrefix) PrintPrefixOut();
            Print(
                exceptionPrefixText,
                $"store settings \"{SettingsSerializer.ConfigFilePath}\" caused by ",
                (ex is IOException ? "IO related" : "unknown"),
                $" issues!{Environment.NewLine}"
            );
            return false;
        }
        return true;
    }

    public void StoreResult(string tabs, DateTime timestamp, string taskName, TimeSpan duration, string programName, string programFilePath, string[] programArgs, IJWAoCHTTPResponse response)
    {
        if (!string.IsNullOrEmpty(Settings.SpecificResultTargetPath) && response.StatusCode == 200)
        {
            if (!File.Exists(Settings.ResultTargetPath) || File.ReadAllText(Settings.ResultTargetPath).Trim().Length == 0)
            {
                File.WriteAllText(Settings.ResultTargetPath, string.Join(';', new string[] { "Timestamp", "Task", "Result", "Duration", "Program", "Path", "Request", "Response" }));
            }

            PrintPrefixOut();
            Print($"{tabs}store result... ");
            try
            {
                File.AppendAllText(Settings.ResultTargetPath, Environment.NewLine + string.Join(';', new string[] {
                    timestamp.ToString("yyyy.MM.dd HH:mm:ss:fff"),
                    taskName,
                    response.StatusCode == 200 ? response.Content.ToString() : "null",
                    duration.ToString(),
                    programName,
                    programFilePath,
                    string.Join(" ", programArgs),
                    response.ToString(true),
                }));
                Print($"was successful!{Environment.NewLine}");
            }
            catch
            {
                Print($"failed!{Environment.NewLine}");
            }
        }
    }

    // get-methods
    public IList<string> GetSourceFilePaths(string[] sourcePaths, Regex regex)
    {
        bool AllowedFilePath(string filePath)
        {
            return regex.Match(filePath).Success ||
                (CurrentYear == null || new Regex(@"^.*[^\d]+" + CurrentYear.ToString() + @"[^\d]+.*$").Match(filePath).Success) ||
                (CurrentDay == null || new Regex(@"^.*[^\d]+0*" + CurrentDay.ToString() + @"[^\d]+.*$").Match(filePath).Success) ||
                (CurrentSub == null || new Regex(@"^.*[^\w]+" + CurrentSub + @"[^\w]+.*$").Match(filePath).Success);
        }
        return IOService.GetSourceFilePaths(AllowedFilePath, sourcePaths)
            .Select(s => Tuple.Create(
                regex.Match(s).Success,
                (CurrentYear != null && new Regex(@"^.*[^\d]+" + CurrentYear.ToString() + @"[^\d]+.*$").Match(s).Success),
                (CurrentDay != null && new Regex(@"^.*[^\d]+0*" + CurrentDay.ToString() + @"[^\d]+.*$").Match(s).Success),
                (CurrentSub != null && new Regex(@"^.*[^\w]+" + CurrentSub + @"[^\w]+.*$").Match(s).Success),
                s
            ))
            .OrderByDescending(e => e.Item1).ThenByDescending(e => e.Item2).ThenByDescending(e => e.Item3).ThenByDescending(e => e.Item4).ThenBy(e => e.Item5)
            .Select(e => e.Item5)
            .ToList();
    }

    // print-methods
    public override void PrintPrefixIn()
    {
        Console.Write(PROGRAM_NAME_SHORT);
        PrintPrefixLevel();
        Console.Write("> ");
        Console.Write(new string(' ', 2 * printLevel));
    }

    public override void PrintPrefixOut()
    {
        Console.Write('<');
        Console.Write(PROGRAM_NAME_SHORT);
        PrintPrefixLevel();
        Console.Write(new string(' ', 2 * printLevel + 1));
    }

    protected void PrintPrefixLevel()
    {
        if (CurrentYear != null)
        {
            Console.Write(':');
            Console.Write(CurrentYear);
            if (CurrentDay != null)
            {
                Console.Write('/');
                if (CurrentDay < 10)
                {
                    Console.Write('0');
                }
                Console.Write(CurrentDay);
                if (CurrentSub != null)
                {
                    Console.Write('/');
                    Console.Write(CurrentSub);
                }
            }
        }
    }
}