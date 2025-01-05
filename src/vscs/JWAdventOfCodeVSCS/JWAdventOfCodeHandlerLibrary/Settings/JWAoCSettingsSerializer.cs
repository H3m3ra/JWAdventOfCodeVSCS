using System.Text.Json;

namespace JWAdventOfCodeHandlerLibrary.Settings;

public class JWAoCSettingsSerializer<T> where T : IJWAoCSettings
{
    public virtual string ConfigFilePath { get; set; } = null!;

    public virtual string ProgramVersionIdentifier { get; set; } = null!;

    public JWAoCSettingsSerializer(string configFilePath, string programVersionIdentifier)
    {
        ConfigFilePath = configFilePath;
        ProgramVersionIdentifier = programVersionIdentifier;
    }

    // load-methods
    /// <summary>
    /// Load the settings from file path <c>CONFIG_FILE_PATH</c> from key <c>PROGRAM_VERSION</c>.
    /// </summary>
    /// <returns>The serialized stored settings.</returns>
    public virtual T? LoadSettings()
    {
        if (!File.Exists(ConfigFilePath))
        {
            var currentSettings = (T?)Activator.CreateInstance(typeof(T));
            if (currentSettings != null)
            {
                StoreSettings(currentSettings);
            }
        }

        var currentConfig = JsonSerializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(ConfigFilePath));
        T? settings;
        if (currentConfig != null && currentConfig.ContainsKey(ProgramVersionIdentifier))
        {
            settings = JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(currentConfig[ProgramVersionIdentifier]));
        }
        else
        {
            settings = (T?)Activator.CreateInstance(typeof(T));
            if (settings != null)
            {
                StoreSettings(settings);
            }
        }

        return settings;
    }

    // store-methods
    /// <summary>
    /// Store the settings at file path <c>CONFIG_FILE_PATH</c> at key <c>PROGRAM_VERSION</c>.
    /// </summary>
    /// <param name="settings">The settings to store.</param>
    public virtual void StoreSettings(T settings)
    {
        Dictionary<string, object> currentConfig = new Dictionary<string, object>();

        if (File.Exists(ConfigFilePath))
        {
            currentConfig = JsonSerializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(ConfigFilePath)) ?? currentConfig;
        }

        if (!currentConfig.ContainsKey(ProgramVersionIdentifier))
        {
            currentConfig.Add(ProgramVersionIdentifier, settings);
        }

        File.WriteAllText(ConfigFilePath, JsonSerializer.Serialize(currentConfig, new JsonSerializerOptions { WriteIndented = true }));
    }
}