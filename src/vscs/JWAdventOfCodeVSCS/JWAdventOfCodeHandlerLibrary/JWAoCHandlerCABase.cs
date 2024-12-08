using JWAdventOfCodeHandlerLibrary.Settings;

namespace JWAdventOfCodeHandlerLibrary;

public abstract class JWAoCHandlerCABase<T> : JWAoCCABase where T : IJWAoCSettings
{
    protected JWAoCSettingsSerializer<T> SettingsSerializer { get; set; } = null!;

    public T Settings { get; set; }
}