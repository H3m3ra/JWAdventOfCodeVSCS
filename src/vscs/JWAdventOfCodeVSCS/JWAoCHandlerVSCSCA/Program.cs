using JWAdventOfCodeHandlerLibrary;
using JWAdventOfCodeHandlerLibrary.Command;
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
    currentAoCVSCS.CommandHandlers.Add(new JWAoCCallCommandHandler() { Handler = currentAoCVSCS });
    currentAoCVSCS.CommandHandlers.Add(new JWAoCChangeCommandHandler() { Handler = currentAoCVSCS });
    currentAoCVSCS.CommandHandlers.Add(new JWAoCCurrentCommandHandler() { Handler = currentAoCVSCS });
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