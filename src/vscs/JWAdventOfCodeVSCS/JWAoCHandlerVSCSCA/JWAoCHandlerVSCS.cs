using JWAdventOfCodeHandlerLibrary;
using JWAdventOfCodeHandlerLibrary.Command;
using JWAdventOfCodeHandlerLibrary.Data;
using JWAdventOfCodeHandlerLibrary.Services;
using JWAdventOfCodeHandlerLibrary.Settings;
using JWAdventOfCodeHandlerLibrary.Settings.Program;
using JWAdventOfCodeHandlingLibrary.HTTP;
using JWAoCHandlerVSCSCA.Commands.StringCommands;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace JWAoCHandlerVSCSCA;

public class JWAoCHandlerVSCS : JWAoCHandlerCABase<JWAoCVSCSSettings>
{
    public const string PROGRAM_NAME_FULL = "JWAdventOfCodeVSCSCA";
    public const string PROGRAM_VERSION_FULL = "1.0.0.20241203201300";
    public const string PROGRAM_NAME = "JWAoCVSCS";
    public const string PROGRAM_VERSION = "v1.0";
    public const string PROGRAM_NAME_SHORT = "AoCVSCS";

    public static readonly Regex TASK_REGEX = new Regex("task", RegexOptions.IgnoreCase);
    public static readonly Regex INPUT_REGEX = new Regex("input", RegexOptions.IgnoreCase);
    public static readonly Regex TEST_REGEX = new Regex("test", RegexOptions.IgnoreCase);

    public const string TASK_SUFFIX = "_task.txt";
    public const string INPUT_SUFFIX = "_input.txt";
    public const string TEST_SUFFIX = "_test.txt";

    public IJWAoCIOService IOService { get; set; }
    public IJWAoCProgramExecutionService ProgramExecutionService { get; set; }
    public IJWAoCResultHandlerService ResultHandlerService { get; set; }

    public int? CurrentYear { get; set; } = null;
    public int? CurrentDay { get; set; } = null;
    public string? CurrentSub { get; set; } = null;

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
                foreach (var line in File.ReadLines(args[0]))
                {
                    ExecuteExternCommand(line);
                }
            }
            catch
            {
                Print($"Cannot execute file from path \"{args[1]}\"!{Environment.NewLine}");
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
        return ExecuteCommand(GetCommandOfString(source));
    }

    public override bool ExecuteCommand(IJWAoCCommand command)
    {
        if (command == null) return true;

        if (command is JWAoCCallCommand)
        {
            CurrentYear = 2024;
            CurrentDay = 1;
            CurrentSub = "a";
            return ((JWAoCCallCommand)command).Execute(Settings, this);
        }
        else if (command is JWAoCGetCommand)
        {
            var currentCommand = (JWAoCGetCommand)command;
            if(LoadSettrings("  Cannot ", true))
            {
                PrintLinesOut(currentCommand.GetValues(Settings, ProgramExecutionService).ToArray());
            }
            return true;
        }
        else if(command is JWAoCSetCommand)
        {
            var currentCommand = (JWAoCSetCommand)command;
            Settings = currentCommand.SetValues(SettingsSerializer.LoadSettings());
            SettingsSerializer.StoreSettings(Settings);
            return true;
        }
        else if (command is JWAoCSimpleStringCommand)
        {
            var currentCommand = (JWAoCSimpleStringCommand)command;

            if (currentCommand.Name.StartsWith("?") || currentCommand.Name.StartsWith("h"))
            {
                Help();
            }
            else if (currentCommand.Name.StartsWith("ch") || currentCommand.Name.StartsWith("y"))
            {
                var parts = currentCommand.Args;
                if (parts.Length == 0)
                {
                    if (CurrentYear != null)
                    {
                        if (CurrentDay != null)
                        {
                            if (CurrentSub != null)
                            {
                                CurrentSub = null;
                            }
                            else
                            {
                                CurrentDay = null;
                            }
                        }
                        else
                        {
                            CurrentYear = null;
                        }
                    }
                }
                else
                {
                    try
                    {
                        var value = int.Parse(parts[0]);
                        if (CurrentYear != null && (value >= 1 && value <= 31))
                        {
                            CurrentDay = value;
                        }
                        else
                        {
                            CurrentYear = value;
                            if (CurrentYear < 100)
                            {
                                var currentFullYear = DateTime.Now.Year;
                                var currentShortYear = currentFullYear % 100;
                                currentFullYear -= currentShortYear;
                                if (CurrentYear > currentShortYear)
                                {
                                    currentFullYear -= 100;
                                }
                                CurrentYear += currentFullYear;
                            }
                        }
                    }
                    catch
                    {
                        if (CurrentYear != null)
                        {
                            var value = parts[0].Trim();
                            string currentSub = "";
                            if (value.Length >= 2 && Char.IsDigit(value[1]))
                            {
                                CurrentDay = int.Parse(value.Substring(0, 2));
                                currentSub = value.Substring(2).Trim();
                            }
                            else if (Char.IsDigit(value[0]))
                            {
                                CurrentDay = int.Parse(value.Substring(0, 1));
                                currentSub = value.Substring(1).Trim();
                            }
                            else
                            {
                                currentSub = value;
                            }

                            if (CurrentDay != null)
                            {
                                CurrentSub = (currentSub.Length == 0 ? null : currentSub);
                            }
                        }
                    }
                }
            }
            else if (currentCommand.Name.StartsWith("cr"))
            {
                var taskFilePath = GetSourceFilePaths(new string[] { Settings.TasksSourcePath }, new Regex("task", RegexOptions.IgnoreCase)).FirstOrDefault();
                if (string.IsNullOrEmpty(taskFilePath))
                {
                    taskFilePath = $"{Settings.TasksSourcePath}{Path.DirectorySeparatorChar}{CurrentYear}{Path.DirectorySeparatorChar}{(CurrentDay < 10 ? "0" : "")}{CurrentDay}{CurrentSub}_task.txt";
                }

                IList<string> lines = new List<string>();
                string line = null;
                while ((line = Console.ReadLine()).Length > 0 || lines.Count < 1 || lines.Last().Length > 0)
                {
                    lines.Add(line);
                }
                lines.RemoveAt(lines.Count - 1);
                File.WriteAllText(taskFilePath, string.Join(Environment.NewLine, lines));
            }
            else if (currentCommand.Name.StartsWith("sh"))
            {
                PrintLinesOut(GetSourceFilePaths(new string[] { Settings.TasksSourcePath }, TASK_REGEX).ToArray());
                PrintLinesOut(GetSourceFilePaths(new string[] { Settings.InputsSourcePath }, INPUT_REGEX).ToArray());
                PrintLinesOut(GetSourceFilePaths(new string[] { Settings.TestsSourcePath }, TEST_REGEX).ToArray());
            }
            return true;
        }
        else
        {
            return false;
        }
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
                (CurrentYear == null || filePath.Contains(CurrentYear.ToString())) ||
                (CurrentDay == null || filePath.Contains(CurrentDay.ToString())) ||
                (CurrentSub == null || filePath.Contains(CurrentSub));
        }
        var x = IOService.GetSourceFilePaths(AllowedFilePath, sourcePaths).Select(s => Tuple.Create(
                regex.Match(s).Success,
                CurrentYear == null || s.Contains(CurrentYear.ToString()),
                CurrentDay == null || s.Contains(CurrentDay.ToString()),
                CurrentSub == null || s.Contains(CurrentSub),
                s
            ))
            .OrderByDescending(e => e.Item1).ThenByDescending(e => e.Item2).ThenByDescending(e => e.Item3).ThenByDescending(e => e.Item4).ThenBy(e => e.Item5)
            ;
        return IOService.GetSourceFilePaths(AllowedFilePath, sourcePaths)
            .Select(s => Tuple.Create(
                regex.Match(s).Success,
                CurrentYear == null || s.Contains(CurrentYear.ToString()),
                CurrentDay == null || s.Contains(CurrentDay.ToString()),
                CurrentSub == null || s.Contains(CurrentSub),
                s
            ))
            .OrderByDescending(e => e.Item1).ThenByDescending(e => e.Item2).ThenByDescending(e => e.Item3).ThenByDescending(e => e.Item4).ThenBy(e => e.Item5)
            .Select(e => e.Item5)
            .ToList();
    }

    protected IJWAoCCommand GetCommandOfString(string source)
    {
        if (string.IsNullOrEmpty(source)) return null;

        var trimmedSource = source.Trim();
        var simpleSource = source.ToLower();

        if (simpleSource.StartsWith("se")) return JWAoCSetCommand.ToSetCommandFromString(source);
        if (simpleSource.StartsWith("?") || simpleSource.StartsWith("h")) 
        {
            
        }
        else if (simpleSource.StartsWith("ca")) return JWAoCCallCommand.ToCallCommandFromString(source);
        else if (simpleSource.StartsWith("cr"))
        {

        }
        else if (simpleSource.StartsWith("ch"))
        {

        }
        else if (simpleSource.StartsWith("g")) return JWAoCGetCommand.ToGetCommandFromString(source);
        else if (simpleSource.StartsWith("se")) return JWAoCSetCommand.ToSetCommandFromString(source);
        else if (simpleSource.StartsWith("sh"))
        {
        
        }
        else if (simpleSource.StartsWith("y"))
        {
        
        }
        
        return null;
    }

    // print-methods
    public override void PrintPrefixIn()
    {
        Console.Write(PROGRAM_NAME_SHORT);
        PrintPrefixLevel();
        Console.Write("> ");
    }

    public override void PrintPrefixOut()
    {
        Console.Write('<');
        Console.Write(PROGRAM_NAME_SHORT);
        PrintPrefixLevel();
        Console.Write(' ');
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