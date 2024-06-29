using System.Collections.Generic;
using BepInEx.Configuration;

namespace TerminalFormatter
{
    public class ConfigManager
    {
        public static ConfigManager Instance { get; private set; }

        public static void Init(ConfigFile config)
        {
            Instance = new ConfigManager(config);
        }

        internal static ConfigFile configFile;

        public static ConfigEntry<bool> ShowDifficultyInAll { get; private set; }

        public static ConfigEntry<bool> DetailedScanPage { get; private set; }

        public static ConfigEntry<int> DivideShopPage { get; private set; }

        public static ConfigEntry<bool> UseShortenedWeathers { get; private set; }
        public static ConfigEntry<bool> AlwaysDisplayHiddenMoons { get; private set; }
        public static ConfigEntry<bool> ShowNumberedPlanetNames { get; private set; }

        public static ConfigEntry<bool> ShowDecorations { get; private set; }
        public static ConfigEntry<bool> ShowHelpText { get; private set; }

        public static ConfigEntry<string> LastUsedPreview { get; private set; }
        public static ConfigEntry<string> LastUsedFilter { get; private set; }
        public static ConfigEntry<string> LastUsedSort { get; private set; }

        public static ConfigEntry<int> LinesToScroll { get; private set; }

        private ConfigManager(ConfigFile config)
        {
            configFile = config;

            ShowDifficultyInAll = configFile.Bind(
                "General",
                "Show Difficulty in All",
                false,
                "Show difficulty in `preview all` setting"
            );

            DetailedScanPage = configFile.Bind(
                "General",
                "Detailed Scan Page",
                true,
                "Enable detailed scan page"
            );

            UseShortenedWeathers = configFile.Bind(
                "Moons",
                "Use Shortened Weathers",
                false,
                "Use shortened weathers in moons catalogue"
            );

            ShowNumberedPlanetNames = configFile.Bind(
                "General",
                "Show Numbered Planet Names",
                false,
                "Show numbered planet names in terminal"
            );

            AlwaysDisplayHiddenMoons = configFile.Bind(
                "Moons",
                "Always Display Hidden Moons",
                false,
                "Always display hidden moons in moons catalogue"
            );

            ShowDecorations = configFile.Bind(
                "General",
                "Show Decorations",
                false,
                "Show decorations in terminal"
            );

            ShowHelpText = configFile.Bind(
                "General",
                "Show Help Text",
                true,
                "Show help text in terminal"
            );

            DivideShopPage = configFile.Bind(
                "Store",
                "Divide shop page into groups",
                5,
                "Number of items per shop section (set to 0 to disable)"
            );

            LastUsedPreview = configFile.Bind(
                "Last Used LLL Option",
                "Last Used Preview",
                "All",
                "Last used preview setting"
            );

            LastUsedFilter = configFile.Bind(
                "Last Used LLL Option",
                "Last Used Filter",
                "None",
                "Last used filter setting"
            );

            LastUsedSort = configFile.Bind(
                "Last Used LLL Option",
                "Last Used Sort",
                "Price",
                "Last used sort setting"
            );

            LinesToScroll = configFile.Bind(
                "Scrolling",
                "Lines to Scroll",
                20,
                "Number of lines to scroll per mouse wheel tick"
            );
        }
    }
}
