using Surveill.DeploymentUtility.App.Settings;
using System.Text.Json;

namespace Surveill.DeploymentUtility.App.Services;

public class AppSettingsManager
{
    public           AppSettings? AppSettings { get; private set; } = new();

    private readonly string      _appSettingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

    public event Action? AppSettingsChanged;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true
    };

    public Task SaveAsync() =>
        File.WriteAllTextAsync(_appSettingsPath,
                               JsonSerializer.Serialize(AppSettings, _jsonSerializerOptions))
            .ContinueWith(_ => AppSettingsChanged?.Invoke());

    public async Task<AppSettings?> LoadAsync()
    {
        if (!File.Exists(_appSettingsPath))
        {
            return AppSettings;
        }

        await using var fi = File.OpenRead(_appSettingsPath);
        AppSettings = await JsonSerializer.DeserializeAsync<AppSettings>(fi, _jsonSerializerOptions);
        return AppSettings;
    }
}