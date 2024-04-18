using System.Collections.Generic;
using System.Linq;

namespace TerminalFormatter
{
    public class SharedMethods
    {
        public static string GetWeather(SelectableLevel level)
        {
            if (Plugin.isLLLPresent)
            {
                return LLLMethods.GetWeather(level);
            }
            // TODO add weathertweaks it's my own fucking mod and i cannot get it to work lol
            else if (Plugin.isWTPresent)
            {
                return WeatherTweaksCompatibility.CurrentWeather(level);
            }
            else
            {
                return level.currentWeather.ToString();
            }
        }

        public static string GetNumberlessPlanetName(SelectableLevel level)
        {
            return new string(level.PlanetName.SkipWhile(c => !char.IsLetter(c)).ToArray());
        }

        public static List<SelectableLevel> GetGameLevels()
        {
            if (Plugin.isLLLPresent)
            {
                return LLLMethods.GetLevels();
            }
            else
            {
                return StartOfRound.Instance.levels.ToList();
            }
        }

        public static int GetPrice(int beforeDiscountPrice)
        {
            Plugin.logger.LogWarning($"price: {beforeDiscountPrice}");

            if (Plugin.isLGUPresent)
            {
                Plugin.logger.LogInfo($"LGU is present");
                return LategameUpgradesCompatibility.GetMoonPrice(beforeDiscountPrice);
            }
            else
            {
                return beforeDiscountPrice;
            }
        }
    }
}