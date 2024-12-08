using JWAdventOfCodeHandlerLibrary;
using JWAdventOfCodeHandlerLibrary.Command;
using JWAdventOfCodeHandlerLibrary.Services;
using JWAdventOfCodeHandlerLibrary.Settings;
using JWAdventOfCodeHandlerLibrary.Settings.Program;
using JWAdventOfCodeHandlingLibrary.HTTP;
using JWAoCHandlerVSCSCA.Commands.StringCommands;
using System.Diagnostics;
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

    public const string TASK_SUFFIX = "_task.txt";
    public const string INPUT_SUFFIX = "_input.txt";
    public const string TEST_SUFFIX = "_test.txt";

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

        if (!Silent) Console.Write($"{PROGRAM_NAME} {PROGRAM_VERSION} starting...");
        try
        {
            Settings = SettingsSerializer.LoadSettings();
        }
        catch
        {
            if (!Silent) Console.WriteLine($" cannot load settings \"{SettingsSerializer.ConfigFilePath}\"!");
            return false;
        }

        if (!Settings.Init())
        {
            if (!Silent) Console.WriteLine(" cannot initalize settings!");
            return false;
        }

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
                if (!Silent) Console.WriteLine($"Cannot execute file from path \"{args[1]}\"!");
            }
        }

        if (!Silent) Console.WriteLine();
        return true;
    }

    public override void Dispose()
    {
        if (!Silent) Console.WriteLine($"...{PROGRAM_NAME} finished.");
    }

    // methods
    protected override bool ExecuteCommand(string source)
    {
        if (String.IsNullOrEmpty(source)) return true;
        return ExecuteCommand(GetCommandOfString(source));
    }

    public override bool ExecuteCommand(IJWAoCCommand command)
    {
        if (command == null) return true;

        if (command is JWAoCCallCommand)
        {
            var currentCommand = (JWAoCCallCommand)command;
            Settings = SettingsSerializer.LoadSettings();
            CurrentYear = 2024;
            CurrentDay = 1;
            CurrentSub = "a";
            if (Settings.Programs.ContainsKey(currentCommand.ProgramName))
            {
                var inputFilePath = GetSourceFilePaths(Settings.InputsSourcePath, "_input.txt", (CurrentDay == null ? null : CurrentDay.ToString()), CurrentSub).FirstOrDefault();
                if (String.IsNullOrEmpty(inputFilePath))
                {
                    inputFilePath = GetSourceFilePaths(Settings.InputsSourcePath, "_input.txt", (CurrentDay == null ? null : CurrentDay.ToString())).FirstOrDefault();
                }

                if (Settings.Programs[currentCommand.ProgramName].ProgramType == JWAoCProgramType.EXE)
                {
                    var args = currentCommand.GetSolveCallArgs("v1", (int)CurrentYear, (int)CurrentDay, CurrentSub, inputFilePath);
                    PrintLineOut($"  \"{currentCommand.ProgramName}\" with \"{String.Join(" ", args)}\" starting...");

                    var start = DateTime.Now;
                    var result = ExecuteProgramWithHTTPGet(currentCommand.ProgramName, "/versions");
                    var referenceDuration = DateTime.Now - start;

                    start = DateTime.Now;
                    result = ExecuteProgram(currentCommand.ProgramName, args);
                    var duration = DateTime.Now - start;

                    PrintLineOut($"  ...\"{currentCommand.ProgramName}\" finished. ({duration})");
                    if (!String.IsNullOrEmpty(Settings.ResultTargetPath))
                    {
                        PrintLineOut($"  store result...");
                    }
                    PrintLineOut($"  {result}");
                }
            }
            return true;
        }
        else if (command is JWAoCGetCommand)
        {
            var currentCommand = (JWAoCGetCommand)command;
            Settings = SettingsSerializer.LoadSettings();
            PrintLinesOut(currentCommand.GetValues(this).ToArray());
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
                string filePath = GetSourceFilePaths(
                    Settings.TasksSourcePath,
                    $"{(CurrentDay < 10 ? "0" : "")}{CurrentDay}{CurrentSub}_task.txt"
                ).FirstOrDefault();
                if (String.IsNullOrEmpty(filePath))
                {
                    filePath = $"{Settings.TasksSourcePath}{Path.DirectorySeparatorChar}{CurrentYear}{Path.DirectorySeparatorChar}{(CurrentDay < 10 ? "0" : "")}{CurrentDay}{CurrentSub}_task.txt";
                }

                IList<string> lines = new List<string>();
                string line = null;
                while ((line = Console.ReadLine()).Length > 0 || lines.Count < 1 || lines.Last().Length > 0)
                {
                    lines.Add(line);
                }
                lines.RemoveAt(lines.Count - 1);
                File.WriteAllText(filePath, String.Join(Environment.NewLine, lines));
            }
            else if (currentCommand.Name.StartsWith("sh"))
            {
                if (CurrentYear == null)
                {
                    PrintLinesOut(GetSourceFilePaths(Settings.TasksSourcePath).ToArray());
                    PrintLinesOut(GetSourceFilePaths(Settings.InputsSourcePath).ToArray());
                    PrintLinesOut(GetSourceFilePaths(Settings.TestsSourcePath).ToArray());
                }
                else
                {
                    PrintLinesOut(GetSourceFilePaths(Settings.TasksSourcePath + Path.DirectorySeparatorChar + CurrentYear).ToArray());
                    PrintLinesOut(GetSourceFilePaths(Settings.InputsSourcePath + Path.DirectorySeparatorChar + CurrentYear).ToArray());
                    PrintLinesOut(GetSourceFilePaths(Settings.TestsSourcePath + Path.DirectorySeparatorChar + CurrentYear).ToArray());
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public object ExecuteProgramWithHTTPGet(string programName, string currentHTTPURIString)
    {
        return ExecuteProgram(programName, "http", "GET", currentHTTPURIString);
    }

    public object ExecuteProgram(string programName, params string[] args)
    {
        if (!File.Exists(Settings.Programs[programName].ProgramFilePath))
        {
            return new JWAoCHTTPResponse(404, new JWAoCHTTPProblemDetails("Program is not available!", 404));
        }

        var startInfo = new ProcessStartInfo();
        startInfo.Arguments = String.Join(' ', args);
        startInfo.CreateNoWindow = false;
        startInfo.FileName = Settings.Programs[programName].ProgramFilePath;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;
        startInfo.UseShellExecute = false;
        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        try
        {
            IList<string> currentHTTPHeaders = new List<string>();
            string currentHTTPBodyText = null;

            using (Process program = Process.Start(startInfo))
            {
                using (var sr = program.StandardOutput)
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (String.IsNullOrEmpty(line))
                        {
                            currentHTTPBodyText = "";
                        }
                        else if (currentHTTPBodyText == null)
                        {
                            currentHTTPHeaders.Add(line);
                        }
                        else
                        {
                            currentHTTPBodyText += line;
                        }
                    }
                }
                program.WaitForExit();
            }

            int currentHTTPStatus = int.Parse(new Regex("\\d\\d\\d").Match(currentHTTPHeaders.First()).Value);
            return new JWAoCHTTPResponse(currentHTTPStatus, String.IsNullOrEmpty(currentHTTPBodyText) ? null : JsonSerializer.Deserialize<object>(currentHTTPBodyText));
        }
        catch(Exception ex)
        {
            return new JWAoCHTTPResponse(500, new JWAoCHTTPProblemDetails(ex.Message, 500));
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

    // get-methods
    public IList<string> GetSourceFilePaths(string sourcePath, string suffix=".txt", string? currentDay=null, string? currentSub=null)
    {
        bool AllowedFilePath(string filePath)
        {
            return (
                    filePath.ToLower().EndsWith(TASK_SUFFIX) ||
                    filePath.ToLower().EndsWith(INPUT_SUFFIX) ||
                    filePath.ToLower().EndsWith(TEST_SUFFIX)
                ) &&
                filePath.ToLower().EndsWith(suffix) &&
                (currentDay == null || filePath.ToLower().Contains(currentDay)) &&
                (currentSub == null || filePath.ToLower().Contains(currentSub));
        }

        return JWAoCFileService.GetSourceFilePaths(sourcePath, AllowedFilePath);
    }

    protected IJWAoCCommand GetCommandOfString(string source)
    {
        if (String.IsNullOrEmpty(source)) return null;

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
    protected override void PrintPrefixIn()
    {
        Console.Write(PROGRAM_NAME_SHORT);
        PrintPrefixLevel();
        Console.Write("> ");
    }

    protected override void PrintPrefixOut()
    {
        Console.Write('<');
        Console.Write(PROGRAM_NAME_SHORT);
        PrintPrefixLevel();
        Console.Write(' ');
    }

    private void PrintPrefixLevel()
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