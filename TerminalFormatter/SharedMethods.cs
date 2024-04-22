using System.Collections.Generic;
using System.Linq;
using MrovLib;

namespace TerminalFormatter
{
    public class SharedMethods
    {
        public static string GetWeather(SelectableLevel level)
        {
            return MrovLib.API.SharedMethods.GetWeather(level);
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
            // Plugin.logger.LogWarning($"price: {beforeDiscountPrice}");

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
