using JWAdventOfCodeHandlerLibrary.Settings.Program;
using JWAdventOfCodeHandlingLibrary.Services;
using System.Text.Json.Serialization;

namespace JWAdventOfCodeHandlerLibrary.Settings;

public abstract class JWAoCSettingsBase : IJWAoCSettings
{
    [JsonPropertyName("task_type")]
    public string TaskType { get; set; } = "task";
    [JsonPropertyName("tasks_src")]
    public string[] TasksSourcePaths { get; set; } = new string[] { "." };
    [JsonPropertyName("tasks_trg")]
    public string TasksTargetPathPattern { get; set; } = @".\%yyyy%\%dd%%s%_%tttt%.txt";

    [JsonPropertyName("input_type")]
    public string InputType { get; set; } = "input";
    [JsonPropertyName("inputs_src")]
    public string[] InputsSourcePaths { get; set; } = new string[] { "." };
    [JsonPropertyName("inputs_trg")]
    public string InputsTargetPathPattern { get; set; } = @".\%yyyy%\%dd%%s%_%tttt%.txt";

    [JsonPropertyName("test_type")]
    public string TestType { get; set; } = "test";
    [JsonPropertyName("tests_src")]
    public string[] TestsSourcePaths { get; set; } = new string[] { "." };
    [JsonPropertyName("tests_trg")]
    public string TestsTargetPathPattern { get; set; } = @".\%yyyy%\%dd%%s%_%tttt%.txt";

    [JsonPropertyName("result_type")]
    public string ResultType { get; set; } = "result";
    [JsonPropertyName("results_trg")]
    public string ResultsTargetPathPattern { get; set; } = @".\%tttt%s\%yyyy%\%a%.csv";

    [JsonPropertyName("programs")]
    public Dictionary<string, JWAoCProgram> Programs { get; set; } = new Dictionary<string, JWAoCProgram>();

    public JWAoCSettingsBase()
    {

    }

    // static-get-methods
    public static string GetTargetPathFromTargetPathPattern(string targetPathPattern, int taskYear, int taskDay, string subTask, string type, string programName, string programVersion, string programAuthor)
    {
        var yyyy = taskYear.ToString();
        var yyValue = JWAocDateService.GetShortYearOfFullYear(taskYear);
        var yy = (yyValue == null ? yyyy : yyValue.ToString());
        var d = taskDay.ToString();
        var dd = (taskDay < 0 ? "0" : "") + d;

        type ??= "";
        programName ??= "";
        programVersion ??= "";
        programAuthor ??= "";

        var result = targetPathPattern
            .Replace("%yy%", yy).Replace("%yyyy%", yyyy)
            .Replace("%d%", d).Replace("%dd%", dd)
            .Replace("%-s%", subTask.ToLower()).Replace("%s%", subTask).Replace("%+s%", subTask.ToUpper())
            .Replace("%-tttt%", type.ToLower()).Replace("%tttt%", type).Replace("%+tttt%", type.ToUpper())
            .Replace("%-p%", programName.ToLower()).Replace("%p%", programName).Replace("%+p%", programName.ToUpper())
            .Replace("%-v%", programVersion.ToLower()).Replace("%v%", programVersion).Replace("%+v%", programVersion.ToUpper())
            .Replace("%-a%", programAuthor.ToLower()).Replace("%a%", programAuthor).Replace("%+a%", programAuthor.ToUpper());
        for (int t=1;t<=3;t++)
        {
            if (type.Length >= t)
            {
                result = result
                    .Replace("%-" + new string('t', t) + "%", type[..t].ToLower())
                    .Replace("%" + new string('t', t) + "%", type[..t])
                    .Replace("%+" + new string('t', t) + "%", type[..t].ToUpper());
            }
        }
        foreach (var c in Path.GetInvalidPathChars())
        {
            result = result.Replace(c, '_');
        }
        return result;
    }

    // init-methods
    public virtual bool Init()
    {
        return true;
    }

    // get-methods
    public string GetTaskTargetPath(int taskYear, int taskDay, string subTask, string programName, string programVersion, string programAuthor)
    {
        return GetTargetPathFromTargetPathPattern(TasksTargetPathPattern, taskYear, taskDay, subTask, TaskType, programName, programVersion, programAuthor);
    }

    public string GetInputTargetPath(int taskYear, int taskDay, string subTask, string programName, string programVersion, string programAuthor)
    {
        return GetTargetPathFromTargetPathPattern(InputsTargetPathPattern, taskYear, taskDay, subTask, InputType, programName, programVersion, programAuthor);
    }

    public string GetTestTargetPath(int taskYear, int taskDay, string subTask, string programName, string programVersion, string programAuthor)
    {
        return GetTargetPathFromTargetPathPattern(TestsTargetPathPattern, taskYear, taskDay, subTask, TestType, programName, programVersion, programAuthor);
    }

    public string GetResultTargetPath(int taskYear, int taskDay, string subTask, string programName, string programVersion, string programAuthor)
    {
        return GetTargetPathFromTargetPathPattern(TestsTargetPathPattern, taskYear, taskDay, subTask, ResultType, programName, programVersion, programAuthor);
    }
}