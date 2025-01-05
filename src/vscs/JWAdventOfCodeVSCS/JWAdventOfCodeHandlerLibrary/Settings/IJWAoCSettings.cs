using JWAdventOfCodeHandlerLibrary.Settings.Program;

namespace JWAdventOfCodeHandlerLibrary.Settings;

public interface IJWAoCSettings
{
    public string TaskType { get; set; }
    public string[] TasksSourcePaths { get; set; }
    public string TasksTargetPathPattern { get; set; }

    public string InputType { get; set; }
    public string[] InputsSourcePaths{ get; set; }
    public string InputsTargetPathPattern { get; set; }

    public string TestType { get; set; }
    public string[] TestsSourcePaths { get; set; }
    public string TestsTargetPathPattern { get; set; }

    public string ResultType { get; set; }
    public string ResultsTargetPathPattern { get; set; }

    public Dictionary<string, JWAoCProgram> Programs { get; set; }

    public string[] DataTypes { get; }

    // init-methods
    public bool Init();

    // get-methods
    public string GetTaskTargetPath(int taskYear, int taskDay, string subTask, string programName, string programVersion, string programAuthor);

    public string GetInputTargetPath(int taskYear, int taskDay, string subTask, string programName, string programVersion, string programAuthor);

    public string GetTestTargetPath(int taskYear, int taskDay, string subTask, string programName, string programVersion, string programAuthor);

    public string GetResultTargetPath(int taskYear, int taskDay, string subTask, string programName, string version, string author);
}