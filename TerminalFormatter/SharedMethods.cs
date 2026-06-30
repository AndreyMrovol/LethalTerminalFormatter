using System.Collections.Generic;
using System.Text.RegularExpressions;
using TerminalFormatter.Compatibility;

namespace TerminalFormatter
{
  public class SharedMethods
  {
    public static string GetWeather(SelectableLevel level)
    {
      string weather = MrovLib.SharedMethods.GetWeather(level);
      int weatherLength = Settings.planetWeatherWidth - 1;

      Plugin.debugLogger.LogDebug($"Length : {weather.Length}|{weatherLength}");

      if (weather.Length >= weatherLength || ConfigManager.UseShortenedWeathers.Value)
      {
        if (Plugin.WeatherRegistryCompat.IsModPresent)
        {
          return Plugin.WeatherRegistryCompat.GetShortenedWeather(level.currentWeather);
        }
        else
        {
          // display first 3 characters
          return Regex.Replace(weather, @"\s+", "").Substring(0, weatherLength);
        }
      }

      return weather;
    }

    public static string GetNumberlessPlanetName(SelectableLevel level)
    {
      return MrovLib.SharedMethods.GetNumberlessPlanetName(level);
    }

    public static List<SelectableLevel> GetGameLevels()
    {
      return MrovLib.SharedMethods.GetGameLevels();
    }

    public static int GetPrice(int beforeDiscountPrice)
    {
      if (TerminalUtils.Plugin.LGUCompat.IsModPresent)
      {
        return TerminalUtils.Plugin.LGUCompat.GetMoonPrice(beforeDiscountPrice);
      }
      else
      {
        return beforeDiscountPrice;
      }
    }
  }
}
