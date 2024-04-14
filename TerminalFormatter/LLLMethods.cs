using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LethalLevelLoader;

namespace TerminalFormatter
{
    public class LLLMethods
    {
        public static string GetWeather(SelectableLevel level)
        {
            // get ExtendedLevel from SelectableLevel
            ExtendedLevel extendedLevel =
                LethalLevelLoader.PatchedContent.ExtendedLevels.FirstOrDefault(x =>
                    x.selectableLevel == level
                );

            // use reflection to call TerminalManager.GetWeatherConditions - must invoke the original method cause of weathertweaks
            // it's internal static method
            var weatherCondition = typeof(LethalLevelLoader.TerminalManager)
                .GetMethod("GetWeatherConditions", BindingFlags.NonPublic | BindingFlags.Static)
                .Invoke(null, new object[] { extendedLevel })
                .ToString()
                .Replace("(", "")
                .Replace(")", "");

            return weatherCondition;
        }

        public static List<SelectableLevel> GetLevels()
        {
            return LethalLevelLoader
                .PatchedContent.ExtendedLevels.Select(x => x.selectableLevel)
                .ToList();
        }
    }
}
