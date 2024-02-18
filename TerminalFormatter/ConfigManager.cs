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

        private readonly ConfigFile configFile;

        public static ConfigEntry<bool> ShowDifficultyInAll { get; private set; }

        private ConfigManager(ConfigFile config)
        {
            configFile = config;

            ShowDifficultyInAll = configFile.Bind(
                "General",
                "Show Difficulty in All",
                false,
                "Show difficulty in `preview all` setting"
            );
        }
    }
}
