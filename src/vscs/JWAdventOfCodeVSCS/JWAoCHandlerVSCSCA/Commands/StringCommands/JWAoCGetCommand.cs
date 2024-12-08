using JWAdventOfCodeHandlerLibrary.Command;

namespace JWAoCHandlerVSCSCA.Commands.StringCommands;

public class JWAoCGetCommand : JWAoCStringCommandBase
{
    public virtual string PropertyName { get; protected set; }

    public override string[] Args { get { return (PropertyName == null ? new string[] { } : new string[] { PropertyName }); } protected set { } }

    public override string Description { get { return "Get a setting property."; } protected set { } }

    public JWAoCGetCommand()
    {

    }

    // static-to-methods
    public static JWAoCGetCommand ToGetCommandFromString(string source)
    {
        string originalSource = source;

        if (source.Trim().Length == 0) return null;

        int nextIndex;

        source = source.TrimStart();
        nextIndex = (nextIndex = source.IndexOf(' ')) < 0 ? source.Length : nextIndex ;
        var commandName = source.Substring(0, nextIndex);

        source = source.Substring(nextIndex).TrimStart();
        var propertyName = source.TrimEnd();
        if (propertyName.StartsWith("i")) propertyName = "inputs_src";
        else if (propertyName.StartsWith("p")) propertyName = "programs";
        else if (propertyName.StartsWith("results")) propertyName = "results_trg";
        else if (propertyName.StartsWith("result_")) propertyName = "result_trg";
        else if (propertyName.StartsWith("ta")) propertyName = "tasks_src";
        else if (propertyName.StartsWith("te")) propertyName = "tests_src";
        else propertyName = null;

        return new JWAoCGetCommand()
        {
            Name = "get",
            PropertyName = propertyName,
            Source = originalSource
        };
    }

    // get-methods
    public IList<string> GetValues(JWAoCHandlerVSCS handler)
    {
        var settings = handler.Settings;

        if (Args.Length == 0)
        {
            var lines = new List<string>();
            lines.Add("inputs_src:  " + settings.InputsSourcePath);
            lines.Add("results_trg: " + settings.ResultsTargetPath);
            lines.Add("result_trg:  " + settings.ResultTargetPath);
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
                lines.Add($"  \"{entry.Key}\" ({entry.Value.Type}) \"{entry.Value.ProgramFilePath}\"");
                lines.Add($"    type: {entry.Value.Type}");
                lines.Add($"    path: \"{entry.Value.ProgramFilePath}\"");
                if (entry.Value.IsAvailable())
                {
                    lines.Add($"    versions: \"{handler.ExecuteProgramWithHTTPGet(entry.Key, "/versions")}\"");
                    lines.Add($"    author:   \"{handler.ExecuteProgramWithHTTPGet(entry.Key, "/v1/author")}\"");
                    lines.Add($"    version:  \"{handler.ExecuteProgramWithHTTPGet(entry.Key, "/v1/version")}\"");
                }
                else
                {
                    lines.Add($"    NOT AVAILABLE!");
                }
            }
            return lines;
        }
        else if (PropertyName == "results_trg") return new string[] { settings.ResultsTargetPath };
        else if (PropertyName == "results_trg") return new string[] { settings.ResultTargetPath };
        else if (PropertyName == "tasks_src") return new string[] { settings.TasksSourcePath };
        else/*if (PropertyName == "tests_src")*/ return new string[] { settings.TestsSourcePath };
    }
}