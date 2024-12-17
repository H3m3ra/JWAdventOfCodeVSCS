using JWAdventOfCodeHandlerLibrary.Command;
using JWAdventOfCodeHandlerLibrary.Services;
using JWAdventOfCodeHandlerLibrary.Settings;
using JWAdventOfCodeHandlerLibrary.Settings.Program;

namespace JWAoCHandlerVSCSCA.Commands.StringCommands;

public class JWAoCGetCommand : JWAoCStringCommandBase
{
    public virtual string PropertyName { get; set; }

    public override string[] Args { get { return (PropertyName == null ? new string[] { } : new string[] { PropertyName }); } set { } }

    public override string Description { get { return "Get a setting property."; } set { } }

    public JWAoCGetCommand()
    {

    }

    // get-methods
    public IList<string> GetValues(IJWAoCSettings settings, IJWAoCProgramExecutionService programExecutionService)
    {
        if (Args.Length == 0)
        {
            var lines = new List<string>();
            lines.Add("inputs_src:  " + settings.InputsSourcePath);
            lines.Add("results_trg: " + settings.ResultsTargetPath);
            lines.Add("result_trg:  " + settings.SpecificResultTargetPath);
            lines.Add("tasks_src:   " + settings.TasksSourcePath);
            lines.Add("tests_src:   " + settings.TestsSourcePath);
            lines.Add("programs:    ");
            foreach (var entry in settings.Programs)
            {
                lines.Add($"  \"{entry.Key}\" ({entry.Value.Type}) \"{entry.Value.ProgramFilePath}\" {(entry.Value.IsAvailable() ? "Available." : "NOT AVAILABLE!")}");
            }
            return lines;
        }
        else if (PropertyName == "inputs_src") return new string[] { settings.InputsSourcePath };
        else if (PropertyName == "programs")
        {
            var lines = new List<string>();
            lines.Add("programs:    ");
            foreach (var entry in settings.Programs)
            {
                var program = entry.Value;
                lines.Add($"  \"{entry.Key}\" ({program.Type}) \"{program.ProgramFilePath}\"");
                lines.Add($"    type: {program.Type}");
                lines.Add($"    path: \"{program.ProgramFilePath}\"");
                if (entry.Value.IsAvailable())
                {
                    var versions = program.GetVersions(programExecutionService);
                    lines.Add($"    versions: [{string.Join(", ", versions)}]");
                    var version = JWAoCProgram.GetHighestVersionOf(versions);
                    lines.Add($"    version:  \"{version}\"");
                    var result = programExecutionService.CallProgramWithLocalHTTPGet(program, $"/{version}/author");
                    lines.Add($"    author:   {(result.StatusCode == 200 ? $"{result.Content.ToString()}" : $"ERROR {result.StatusCode}: {result.StatusName}")}");
                    result = programExecutionService.CallProgramWithLocalHTTPGet(program, $"/{version}/version");
                    lines.Add($"    version:  {(result.StatusCode == 200 ? $"{result.Content.ToString()}" : $"ERROR {result.StatusCode}: {result.StatusName}")}");
                }
                else
                {
                    lines.Add($"    NOT AVAILABLE!");
                }
            }
            return lines;
        }
        else if (PropertyName == "results_trg") return new string[] { settings.ResultsTargetPath };
        else if (PropertyName == "results_trg") return new string[] { settings.SpecificResultTargetPath };
        else if (PropertyName == "tasks_src") return new string[] { settings.TasksSourcePath };
        else/*if (PropertyName == "tests_src")*/ return new string[] { settings.TestsSourcePath };
    }
}