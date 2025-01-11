using JWAdventOfCodeHandlerLibrary.Command;
using JWAdventOfCodeHandlerLibrary.Services;
using JWAdventOfCodeHandlerLibrary.Settings;

namespace JWAdventOfCodeHandlerLibrary;

public abstract class JWAoCHandlerCABase<T> : IJWAoCCA where T : IJWAoCSettings
{
    public IJWAoCIOService IOService { get; set; } = null!;
    public IJWAoCIOConsoleService IOConsoleService { get; set; } = null!;
    public IJWAoCProgramExecutionService ProgramExecutionService { get; set; } = null!;
    public IJWAoCResultHandlerService ResultHandlerService { get; set; } = null!;
    public IJWAoCSettingsService<T> SettingsService { get; set; } = null!;

    protected JWAoCSettingsSerializer<T> SettingsSerializer { get; set; } = null!;
    public T? Settings { get; set; }

    public bool Interactive { get; set; } = true;
    public bool Silent { get { return IOConsoleService.Silent; } set { IOConsoleService.Silent = value; } }

    // init-methods
    public abstract bool Init(params string[] args);

    public abstract void Dispose();

    // execute-methods
    public bool ExecuteExternCommand(string? source)
    {
        IOConsoleService.PrintLineIn(source ?? string.Empty);
        return ExecuteCommand(source);
    }

    public bool ExecuteConsoleCommand()
    {
        IOConsoleService.PrintPrefixIn();
        return ExecuteCommand(Console.ReadLine());
    }

    protected abstract bool ExecuteCommand(string? source);

    public abstract bool ExecuteCommand(IJWAoCCommand? command);
}