using JWAdventOfCodeHandlerLibrary.Services;
using JWAdventOfCodeHandlerLibrary.Settings;

namespace JWAoCHandlerVSCSCA.Services;

public class JWAoCSettingsService : IJWAoCSettingsService<JWAoCVSCSSettings>
{
    public JWAoCHandlerVSCS Handler { get; set; } = null!;

    // get-methodds
    public JWAoCVSCSSettings? GetSettings()
    {
        return Handler.Settings;
    }
}