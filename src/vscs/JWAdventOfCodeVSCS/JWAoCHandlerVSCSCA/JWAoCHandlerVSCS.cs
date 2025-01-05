using JWAdventOfCodeHandlerLibrary;
using JWAdventOfCodeHandlerLibrary.Command;
using JWAdventOfCodeHandlerLibrary.Handler;
using JWAdventOfCodeHandlerLibrary.Settings;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace JWAoCHandlerVSCSCA;

public class JWAoCHandlerVSCS : JWAoCHandlerCABase<JWAoCVSCSSettings>
{
    public const string PROGRAM_NAME_FULL = "JWAdventOfCodeVSCSCA";
    public const string PROGRAM_VERSION_FULL = "1.1.1.20241223213457";
    public const string PROGRAM_NAME = "JWAoCVSCS";
    public const string PROGRAM_VERSION = "v1.1";
    public const string PROGRAM_AUTHOR = "JWHemera";

    public static readonly Regex TASK_REGEX = new Regex("task", RegexOptions.IgnoreCase);
    public static readonly Regex INPUT_REGEX = new Regex("input", RegexOptions.IgnoreCase);
    public static readonly Regex TEST_REGEX = new Regex("test", RegexOptions.IgnoreCase);

    public IDictionary<string, IJWAoCStringCommandFactory> CommandFactories { get; set; } = new Dictionary<string, IJWAoCStringCommandFactory>();
    public IList<IJWAoCCommandHandler> CommandHandlers { get; set; } = new List<IJWAoCCommandHandler>();

    public int? CurrentYear { get { return currentYear; } set { currentYear = value; UpdateCurrentPath(); } }
    protected int? currentYear = null;
    public int? CurrentDay { get { return currentDay; } set { currentDay = value; UpdateCurrentPath(); } }
    protected int? currentDay = null;
    public string? CurrentSub { get { return currentSub; } set { currentSub = value; UpdateCurrentPath(); } }
    protected string? currentSub = null;

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

        IOConsoleService.Print($"{PROGRAM_NAME} {PROGRAM_VERSION} starting...");
        if (!LoadSettrings(" cannot ")) return false;
        IOConsoleService.Print($"{Environment.NewLine}");

        if (options.Contains("-f") && args.Length >= 2)
        {
            try
            {
                bool execute;
                if (Interactive)
                {
                    IOConsoleService.PrintIn($"Execute file from path \"{args[1]}\"? (y/n) ");
                    execute = IOConsoleService.GetLineIn().Trim().ToLower().StartsWith("y");
                }
                else
                {
                    IOConsoleService.PrintLineOut($"Execute file from path \"{args[1]}\".");
                    execute = true;
                }

                if (execute)
                {
                    IOConsoleService.IndentationLevel = 1;
                    foreach (var line in File.ReadLines(args[1]))
                    {
                        ExecuteExternCommand(line);
                    }
                    IOConsoleService.IndentationLevel = 0;
                }
            }
            catch (Exception ex)
            {
                IOConsoleService.IndentationLevel = 0;
                IOConsoleService.PrintLineOut($"Cannot execute file from path \"{args[1]}\"!");
            }
        }
        return true;
    }

    public override void Dispose()
    {
        IOConsoleService.Print($"...{PROGRAM_NAME} finished.{Environment.NewLine}");
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

        if (handlers.Count == 0)
        {
            IOConsoleService.PrintLineOut($"  Cannot handle command \"{command.GetType().Name}\"!");
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
        IOConsoleService.PrintLinesOut(
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

    // update-methods
    protected void UpdateCurrentPath()
    {
        if (CurrentYear == null)
        {
            IOConsoleService.CurrentPath = "";
        }
        else
        {
            if (CurrentDay == null)
            {
                IOConsoleService.CurrentPath = $":{CurrentYear}";
            }
            else
            {
                if (CurrentSub == null)
                {
                    IOConsoleService.CurrentPath = $":{CurrentYear}/{(CurrentDay < 10 ? "0" : "")}{CurrentDay}";
                }
                else
                {
                    IOConsoleService.CurrentPath = $":{CurrentYear}/{(CurrentDay < 10 ? "0" : "")}{CurrentDay}/{CurrentSub}";
                }
            }
        }
    }

    // load-methods
    public bool LoadSettrings(string exceptionPrefixText, bool printPrefix = false)
    {
        try
        {
            Settings = SettingsSerializer.LoadSettings();
        }
        catch (Exception ex)
        {
            if (printPrefix) IOConsoleService.PrintPrefixOut();
            IOConsoleService.Print(
                exceptionPrefixText,
                $"load settings \"{SettingsSerializer.ConfigFilePath}\" caused by ",
                (ex is IOException ? "IO related" : (ex is JsonException ? "JSON related" : "unknown")),
                $" issues!{Environment.NewLine}"
            );
            return false;
        }
        if (!Settings.Init())
        {
            if (printPrefix) IOConsoleService.PrintPrefixOut();
            IOConsoleService.Print($"{exceptionPrefixText}initalize settings!{Environment.NewLine}");
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
            if (printPrefix) IOConsoleService.PrintPrefixOut();
            IOConsoleService.Print(
                exceptionPrefixText,
                $"store settings \"{SettingsSerializer.ConfigFilePath}\" caused by ",
                (ex is IOException ? "IO related" : "unknown"),
                $" issues!{Environment.NewLine}"
            );
            return false;
        }
        return true;
    }

    // get-methods
    public IList<string> GetSourceFilePaths(string[] sourcePaths, string type)
    {
        bool AllowedFilePath(string filePath)
        {
            return new Regex(type, RegexOptions.IgnoreCase).Match(filePath).Success ||
                (CurrentYear == null || new Regex(@"^.*[^\d]+" + CurrentYear.ToString() + @"[^\d]+.*$").Match(filePath).Success) ||
                (CurrentDay == null || new Regex(@"^.*[^\d]+0*" + CurrentDay.ToString() + @"[^\d]+.*$").Match(filePath).Success) ||
                (CurrentSub == null || new Regex(@"^.*[^\w]+" + CurrentSub + @"[^\w]+.*$").Match(filePath).Success);
        }

        var currentTypeRegex = new Regex(type, RegexOptions.IgnoreCase);
        var oppositeTypeRegex = new Regex(
            string.Join("|", Settings.DataTypes.Where(d => !string.IsNullOrWhiteSpace(d) && d != type)),
            RegexOptions.IgnoreCase
        );

        var results = IOService.GetSourceFilePaths(AllowedFilePath, sourcePaths)
            .Select(s => Tuple.Create(
                currentTypeRegex.Match(s).Success || !oppositeTypeRegex.Match(s).Success,
                (CurrentYear != null && new Regex(@"^.*[^\d]+" + CurrentYear.ToString() + @"[^\d]+.*$").Match(s).Success),
                (CurrentDay != null && new Regex(@"^.*[^\d]+0*" + CurrentDay.ToString() + @"[^\d]+.*$").Match(s).Success),
                (CurrentSub != null && new Regex(@"^.*[^\w]+" + CurrentSub + @"[^\w]+.*$").Match(s).Success),
                s
            ))
            .OrderByDescending(e => e.Item1).ThenByDescending(e => e.Item2).ThenByDescending(e => e.Item3).ThenByDescending(e => e.Item4).ThenBy(e => e.Item5);
        return results.Select(e => e.Item5).ToList();
    }
}