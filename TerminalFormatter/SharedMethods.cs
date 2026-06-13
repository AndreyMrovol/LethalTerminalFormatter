using System.Collections.Generic;
using System.Text.RegularExpressions;
using HarmonyLib;
using TerminalFormatter.Compatibility;

namespace TerminalFormatter
{
  public class SharedMethods
  {
    public static string GetWeather(SelectableLevel level)
    {
      string weather = MrovLib.SharedMethods.GetWeather(level);
      int weatherLength = Settings.planetWeatherWidth - 1;
      bool showDifficulty = false;

      if (Plugin.LLLCompat.IsModPresent)
      {
        showDifficulty = ShouldShowDifficulty(level);
      }

      if (showDifficulty)
      {
        weatherLength -= 7;
      }

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
      if (Plugin.LGUCompat.IsModPresent)
      {
        return LategameUpgradesCompatibility.GetMoonPrice(beforeDiscountPrice);
      }
      else
      {
        return beforeDiscountPrice;
      }
    }

    public static bool ShouldShowDifficulty(SelectableLevel level)
    {
      return (
          ConfigManager.ShowDifficultyInAll.Value && LethalLevelLoader.Settings.levelPreviewInfoType == LethalLevelLoader.PreviewInfoType.All
        )
        || LethalLevelLoader.Settings.levelPreviewInfoType == LethalLevelLoader.PreviewInfoType.Difficulty;
    }

    public static string GetLevelRiskLevel(SelectableLevel level)
    {
      return level.riskLevel;
    }
  }
}
