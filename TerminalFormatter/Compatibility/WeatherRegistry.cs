namespace TerminalFormatter.Compatibility
{
  internal class WeatherRegistryCompatibility : MrovLib.CompatibilityHandler
  {
    public WeatherRegistryCompatibility(string guid, string version = null)
      : base(guid, version) { }

    public string GetShortenedWeather(LevelWeatherType weather)
    {
      return WeatherRegistry.WeatherManager.GetWeather(weather).NameShort;
    }
  }
}
