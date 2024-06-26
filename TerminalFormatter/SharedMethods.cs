using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HarmonyLib;
using MrovLib;

namespace TerminalFormatter
{
    public class SharedMethods
    {
        public static string GetWeather(SelectableLevel level)
        {
            string weather = MrovLib.API.SharedMethods.GetWeather(level);
            int weatherLength = Settings.planetWeatherWidth - 1;
            bool showDifficulty = false;

            if (Plugin.isLLLPresent)
            {
                showDifficulty = ShouldShowDifficulty(level);
            }

            if (showDifficulty)
            {
                weatherLength -= 7;
            }

            if (weather.Length >= weatherLength || ConfigManager.UseShortenedWeathers.Value)
            {
                // weatherCondition =
                //     $"{weatherCondition.Substring(0, Settings.planetWeatherWidth - 2)}..";

                Settings.WeathersShortened.Do(pair =>
                {
                    weather = Regex.Replace(weather, pair.Key, pair.Value);
                    weather = weather.Replace(" ", "");
                });
            }

            return weather;
        }

        public static string GetNumberlessPlanetName(SelectableLevel level)
        {
            return MrovLib.API.SharedMethods.GetNumberlessPlanetName(level);
        }

        public static List<SelectableLevel> GetGameLevels()
        {
            return MrovLib.API.SharedMethods.GetGameLevels();
        }

        public static int GetPrice(int beforeDiscountPrice)
        {
            if (Plugin.isLGUPresent)
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
                    ConfigManager.ShowDifficultyInAll.Value
                    && LethalLevelLoader.Settings.levelPreviewInfoType
                        == LethalLevelLoader.PreviewInfoType.All
                )
                || LethalLevelLoader.Settings.levelPreviewInfoType
                    == LethalLevelLoader.PreviewInfoType.Difficulty;
        }

        public static string GetLevelRiskLevel(SelectableLevel level)
        {
            if (Plugin.LQCompat.IsModPresent)
            {
                string LQRiskLevel = LethalQuantitiesCompatibility.GetLevelRiskLevel(level);
                return LQRiskLevel == null ? level.riskLevel : LQRiskLevel;
            }
            else
            {
                return level.riskLevel;
            }
        }
    }
}
