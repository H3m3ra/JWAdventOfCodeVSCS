using JWAdventOfCodeHandlerLibrary.Command;
using JWAoCHandlerVSCSCA;
using JWAoCHandlerVSCSCA.Commands.StringCommandFactories;
using JWAoCHandlerVSCSCA.Services;

var interactive = false;

using (var currentAoCVSCS = new JWAoCHandlerVSCS() {
    CommandFactories = new Dictionary<string, IJWAoCStringCommandFactory>()
    {
        //{ "?", null},
        //{ "a", null},
        { "c", new JWAoCCallCommandFactory()},
        { "g", new JWAoCGetCommandFactory()},
        //{ "h", null},
        { "s", new JWAoCSetCommandFactory()},
        //{ "sh", null}
    },
    IOService = new JWAoCIOService(),
    ProgramExecutionService = new JWAoCProgramExecutionService(),
    ResultHandlerService = new JWAoCResultCSVHandlerServices()
})
{
    if (currentAoCVSCS.Init(args))
    {
        if (currentAoCVSCS.Interactive)
        {
            while (currentAoCVSCS.ExecuteConsoleCommand()) { }
        }
    }

    interactive = currentAoCVSCS.Interactive;
}

if (interactive)
{
    Console.Write("Enter to continue... ");
    Console.ReadLine();
}