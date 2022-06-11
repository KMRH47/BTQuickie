using System.IO;
using System.Reflection;
using System.Resources;
using System.Text.Encodings.Web;
using System.Text.Json;
using BTQuickie.Models.Settings;

namespace BTQuickie.Services.Settings;

public class ApplicationSettingsProvider : IApplicationSettingsProvider
{
    private const string AppSettingsFileName = "appsettings.json";
    private const string AppSettingsFullName = "Resources.Settings.appsettings.json";
    private const string JsonExceptionMessage = "[Fatal] Could not deserialize JSON.";

    public UserSettings UserSettings { get; } = GetUserSettings();

    /// <summary>
    /// Writes the user settings to the settings file.<br/><br/>
    /// --<br/>
    /// <i>The file is written to the application's directory.</i>
    /// </summary>
    public void WriteUserSettings()
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), AppSettingsFileName);

        JsonSerializerOptions jsonSerializerOptions = new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        string json = JsonSerializer.Serialize(UserSettings, jsonSerializerOptions);
        File.WriteAllText(filePath, json);
    }
    
    /// <summary>
    /// Attempts to retrieve a locally stored user settings file.
    /// </summary>
    /// <returns>User settings if a file is present, otherwise the default application settings.</returns>
    /// <exception cref="JsonException">Invalid JSON.</exception>
    private static UserSettings GetUserSettings()
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), AppSettingsFileName);
        bool fileNotFound = !File.Exists(filePath);

        if (fileNotFound)
        {
            return GetDefaultSettings();
        }

        string json = File.ReadAllText(filePath);
        UserSettings? deserialized = JsonSerializer.Deserialize<UserSettings>(json);

        return deserialized ?? throw new JsonException($"{JsonExceptionMessage}");
    }
    
    /// <summary>
    /// Gets the default user settings.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="MissingManifestResourceException">The resource could not be found.</exception>
    /// <exception cref="JsonException">Invalid JSON.</exception>
    private static UserSettings GetDefaultSettings()
    {
        Assembly executingAssembly = Assembly.GetExecutingAssembly();
        string assemblyName = executingAssembly.GetName().Name!;
        Stream? resourceStream = executingAssembly.GetManifestResourceStream($"{assemblyName}.{AppSettingsFullName}");

        if (resourceStream is null)
        {
            throw new MissingManifestResourceException($"[Fatal] Could not find '{AppSettingsFullName}'.");
        }

        using StreamReader sr = new(resourceStream);
        string json = sr.ReadToEnd();
        UserSettings? deserialized = JsonSerializer.Deserialize<UserSettings>(json);

        return deserialized ?? throw new JsonException($"{JsonExceptionMessage}");
    }
}