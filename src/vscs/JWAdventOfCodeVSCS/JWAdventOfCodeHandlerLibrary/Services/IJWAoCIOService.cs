namespace JWAdventOfCodeHandlerLibrary.Services;

public interface IJWAoCIOService
{
    // save-methods

    // get-methods
    public ISet<string> GetSourceFilePaths(Func<string, bool> allowFilePath, params string[] sourcePaths);
}