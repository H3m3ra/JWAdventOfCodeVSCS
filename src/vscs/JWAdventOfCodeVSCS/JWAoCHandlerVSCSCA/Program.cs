using JWAdventOfCodeHandlerLibrary;
using JWAdventOfCodeHandlerLibrary.Command;
using JWAdventOfCodeHandlerLibrary.Handler;
using JWAoCHandlerVSCSCA;
using JWAoCHandlerVSCSCA.Command.Factories.StringCommandFactories;
using JWAoCHandlerVSCSCA.Handlers.CommandHandlers;
using JWAoCHandlerVSCSCA.Services;

static IJWAoCCA Build()
{
    var currentAoCVSCS = new JWAoCHandlerVSCS()
    {
        CommandFactories = new Dictionary<string, IJWAoCStringCommandFactory>()
        {
            //{ "?", null},
            //{ "a", null},
            { "ca", new JWAoCCallCommandFactory()},
            { "ch", new JWAoCChangeCommandFactory()},
            { "g", new JWAoCGetCommandFactory()},
            //{ "h", null},
            { "s", new JWAoCSetCommandFactory()},
            //{ "sh", null}
        },
        IOService = new JWAoCIOService(),
        IOConsoleService = new JWAoCIOConsoleService() { ProgramName = "AoC" },
        ProgramExecutionService = new JWAoCProgramExecutionService(),
        ResultHandlerService = new JWAoCResultCSVHandlerServices()
    };
    foreach (var commandHandler in new IJWAoCCommandHandler[]{
        new JWAoCCallCommandHandler() { Handler = currentAoCVSCS },
        new JWAoCChangeCommandHandler() { Handler = currentAoCVSCS },
        new JWAoCCurrentCommandHandler() { Handler = currentAoCVSCS }
    })
    {
        currentAoCVSCS.CommandHandlers.Add(commandHandler);
    }

    return currentAoCVSCS;
}

var interactive = false;

using (var currentCA = Build())
{
    if (currentCA.Init(args))
    {
        if (currentCA.Interactive)
        {
            while (currentCA.ExecuteConsoleCommand()) { }
        }
    }

    interactive = currentCA.Interactive;
}

if (interactive)
{
    Console.Write("Enter to continue... ");
    Console.ReadLine();
}