using JWAdventOfCodeHandlerLibrary.Command;

namespace JWAoCHandlerVSCSCA.Commands.StringCommands;

public class JWAoCSetCommand : JWAoCStringCommandBase
{
    public virtual string PropertyName { get; protected set; }

    public virtual string PropertyValue { get; protected set; }

    public override string[] Args { get { return new string[] { PropertyName, PropertyValue }; } protected set { } }

    public override string Description { get { return "Set a setting property."; } protected set { } }

    public JWAoCSetCommand()
    {

    }

    // static-to-methods
    public static JWAoCSetCommand ToSetCommandFromString(string source)
    {
        string originalSource = source;

        if (source.Trim().Length == 0) return null;

        int nextIndex;

        source = source.TrimStart();
        if ((nextIndex = source.IndexOf(' ')) < 0) return null;
        var commandName = source.Substring(0, nextIndex);

        source = source.Substring(nextIndex).TrimStart();
        if ((nextIndex = source.IndexOf(' ')) < 0) return null;
        var propertyName = source.Substring(0, nextIndex).ToLower();
        if (propertyName.StartsWith("i"))
        {
            propertyName = "inputs_src";
        }
        else if (propertyName.StartsWith("results"))
        {
            propertyName = "results_trg";
        }
        else if (propertyName.StartsWith("result_"))
        {
            propertyName = "result_trg";
        }
        else if (propertyName.StartsWith("ta"))
        {
            propertyName = "tasks_src";
        }
        else if (propertyName.StartsWith("te"))
        {
            propertyName = "tests_src";
        }
        else
        {
            return null;
        }

        source = source.Substring(nextIndex).TrimStart();
        var propertyValue = source.TrimEnd();

        return new JWAoCSetCommand()
        {
            Name = "set",
            PropertyName = propertyName.ToLower(),
            PropertyValue = propertyValue,
            Source = originalSource
        };
    }

    // set-methods
    public JWAoCVSCSSettings SetValues(JWAoCVSCSSettings settings)
    {
        if (PropertyName == "inputs_src")
        {
            settings.InputsSourcePath = PropertyValue;
        }
        else if (PropertyName == "results_trg")
        {
            settings.ResultsTargetPath = PropertyValue;
        }
        else if (PropertyName == "results_trg")
        {
            settings.SpecificResultTargetPath = PropertyValue;
        }
        else if (PropertyName == "tasks_src")
        {
            settings.TasksSourcePath = PropertyValue;
        }
        else if (PropertyName == "tests_src")
        {
            settings.TestsSourcePath = PropertyValue;
        }
        return settings;
    }
}