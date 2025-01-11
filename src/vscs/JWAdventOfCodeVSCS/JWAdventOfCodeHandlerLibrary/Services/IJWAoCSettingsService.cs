using JWAdventOfCodeHandlerLibrary.Settings;

namespace JWAdventOfCodeHandlerLibrary.Services;

public interface IJWAoCSettingsService<T> where T : IJWAoCSettings
{
    // get-methods
    public T? GetSettings();
}