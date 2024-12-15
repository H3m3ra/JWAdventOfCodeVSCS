using JWAdventOfCodeHandlerLibrary.Services;
using JWAoCHandlerVSCSCA;

var interactive = false;

using (var currentAoCVSCS = new JWAoCHandlerVSCS() {
    IOService = new JWAoCIOService(),
    ProgramExecutionService = new JWAoCProgramExecutionService()
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