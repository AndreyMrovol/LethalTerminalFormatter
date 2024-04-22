using System.Reflection;
using BepInEx.Bootstrap;

namespace TerminalFormatter
{
    internal class WeatherTweaksCompatibility
    {
        internal static string nspace = "WeatherTweaks";
        internal static MethodInfo GetPlanetCurrentWeather;

        public static void Init()
        {
            // Get the assembly that contains the class
            var assembly = Chainloader.PluginInfos["WeatherTweaks"].Instance.GetType().Assembly;

            // Get the Type object for the class
            var type = assembly.GetType($"{nspace}.Variables");

            if (type != null)
            {
                Plugin.logger.LogInfo($"Type {type} found");

                GetPlanetCurrentWeather = type.GetMethod(
                    "GetPlanetCurrentWeather",
                    BindingFlags.Public | BindingFlags.Static
                );

                if (GetPlanetCurrentWeather != null)
                {
                    Plugin.logger.LogInfo(
                        $"Method {GetPlanetCurrentWeather} found - BetaWeatherTweaks"
                    );
                }
                else
                {
                    Plugin.logger.LogError($"Method {GetPlanetCurrentWeather} not found");

                    // check if the method is internal static string GetPlanetCurrentWeather(SelectableLevel level)
                    // if not, log an error

                    GetPlanetCurrentWeather = type.GetMethod(
                        "GetPlanetCurrentWeather",
                        BindingFlags.NonPublic | BindingFlags.Static
                    );

                    if (GetPlanetCurrentWeather != null)
                    {
                        Plugin.logger.LogInfo($"Method {GetPlanetCurrentWeather} found");
                    }
                    else
                    {
                        Plugin.logger.LogError($"Method {GetPlanetCurrentWeather} not found");
                    }
                }
            }
            else
            {
                Plugin.logger.LogDebug($"Type {nspace}.Variables not found");
            }
        }
    }
}
