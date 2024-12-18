using JWAdventOfCodeHandlerLibrary.Data;
using JWAdventOfCodeHandlerLibrary.Settings;

namespace JWAdventOfCodeHandlerLibrary.Services;

public interface IJWAoCResultHandlerService
{
    // methods
    public void HandleResult(JWAoCResult result, IJWAoCSettings settings, IJWAoCIOConsoleService currentIOConsoleService);
}