using JWAdventOfCodeHandlerLibrary.Services;
using JWAdventOfCodeHandlerLibrary.Settings;
using JWAdventOfCodeHandlerLibrary.Settings.Program;
using System.IO;

namespace JWAoCHandlerVSCSCA.Command.Commands.StringCommands;

public class JWAoCGetCommand : JWAoCStringCommandBase
{
    public virtual string PropertyName { get; set; }

    public override string[] Args { get { return PropertyName == null ? new string[] { } : new string[] { PropertyName }; } set { } }

    public override string Description { get { return "Get a setting property."; } set { } }

    public JWAoCGetCommand()
    {

    }

    // get-methods
    public IList<string> GetValues(IJWAoCSettings settings, IJWAoCProgramExecutionService programExecutionService)
    {
        var lines = new List<string>();

        void AddPaths(string name, IList<string> paths)
        {
            lines.Add(name + (paths.Count() == 1 ? $"\"{paths.First()}\"" : ""));
            if (paths.Count() > 1)
            {
                foreach (string path in paths)
                {
                    lines.Add($"  \"{path}\"");
                }
            }
        }

        if (Args.Length == 0)
        {
            AddPaths("inputs_src:  ", settings.InputsSourcePaths);
            lines.Add($"inputs_trg:  \"{settings.InputsTargetPathPattern}\"");
            lines.Add($"results_trg: \"{settings.ResultsTargetPathPattern}\"");
            AddPaths("tasks_src:   ", settings.TasksSourcePaths);
            lines.Add($"tasks_trg:   \"{settings.TasksTargetPathPattern}\"");
            AddPaths("tests_src:   ", settings.TestsSourcePaths);
            lines.Add($"tests_trg:   \"{settings.TasksTargetPathPattern}\"");
            lines.Add($"programs:");
            foreach (var entry in settings.Programs)
            {
                lines.Add($"  \"{entry.Key}\" ({entry.Value.Type}) \"{entry.Value.ProgramFilePath}\" {(entry.Value.IsAvailable() ? "Available." : "NOT AVAILABLE!")}");
            }
        }
        else if (PropertyName == "inputs_src") AddPaths($"inputs_src: ", settings.InputsSourcePaths);
        else if (PropertyName == "inputs_trg") AddPaths($"inputs_trg: ", settings.InputsSourcePaths);
        else if (PropertyName == "programs")
        {
            lines.Add("programs:    ");
            foreach (var entry in settings.Programs)
            {
                var program = entry.Value;
                lines.Add($"  \"{entry.Key}\" ({program.Type}) \"{program.ProgramFilePath}\"");
                lines.Add($"    type: {program.Type}");
                if (program.ProgramType == JWAoCProgramType.RAW)
                {
                    if (program.ProgramHandler == null)
                    {
                        lines.Add($"    -: No handler settings available!");
                    }
                    else
                    {
                        if (program.ProgramHandler.BuilderFilePath != null) lines.Add($"    builder: \"{program.ProgramHandler.BuilderFilePath}\"");
                        if (program.ProgramHandler.InterpreterFilePath != null) lines.Add($"    interpreter: \"{program.ProgramHandler.InterpreterFilePath}\"");
                        if (program.ProgramHandler.CompilerFilePath != null) lines.Add($"    compiler: \"{program.ProgramHandler.CompilerFilePath}\"");
                    }
                }
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
        }
        else if (PropertyName == "results_trg") lines.Add($"results_trg: \"{settings.ResultsTargetPathPattern}\"");
        else if (PropertyName == "tasks_src") AddPaths("tasks_src: ", settings.TasksSourcePaths);
        else if (PropertyName == "tasks_trg") lines.Add($"tasks_trg: \"{settings.TasksTargetPathPattern}\"");
        else if (PropertyName == "tests_src") AddPaths("tests_src: ", settings.TestsSourcePaths);
        else if (PropertyName == "tests_trg") lines.Add($"tests_trg: \"{settings.TasksTargetPathPattern}\"");

        return lines;
    }
}