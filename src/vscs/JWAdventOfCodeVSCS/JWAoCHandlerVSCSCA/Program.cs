using JWAoCHandlerVSCSCA;

var interactive = false;

using (var currentAoCVSCS = new JWAoCHandlerVSCS())
{
    currentAoCVSCS.Init(args);

    if (currentAoCVSCS.Interactive)
    {
        while (currentAoCVSCS.ExecuteConsoleCommand()) { }
    }

    interactive = currentAoCVSCS.Interactive;
}

if (interactive)
{
    Console.Write("Enter to continue... ");
    Console.ReadLine();
}