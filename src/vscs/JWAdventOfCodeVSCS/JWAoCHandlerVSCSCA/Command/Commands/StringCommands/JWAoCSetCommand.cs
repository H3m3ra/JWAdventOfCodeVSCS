namespace JWAoCHandlerVSCSCA.Command.Commands.StringCommands;

public class JWAoCSetCommand : JWAoCStringCommandBase
{
    public virtual string PropertyName { get; set; }

    public virtual string PropertyValue { get; set; }

    public override string[] Args { get { return new string[] { PropertyName, PropertyValue }; } set { } }

    public override string Description { get { return "Set a setting property."; } set { } }

    public JWAoCSetCommand()
    {

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