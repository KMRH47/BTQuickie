using BTQuickie.Models.Settings;

namespace BTQuickie.Services.Settings;

public interface IApplicationSettingsProvider
{
  UserSettings UserSettings { get; }
  void WriteUserSettings();
}