using BepInEx.Configuration;
using MrovLib;

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

    public static ConfigEntry<LoggingType> LoggingLevels { get; private set; }

    public static ConfigEntry<bool> DetailedScanPage { get; private set; }

    public static ConfigEntry<int> DivideShopPage { get; private set; }

    public static ConfigEntry<bool> UseShortenedWeathers { get; private set; }

    public static ConfigEntry<bool> AlwaysDisplayHiddenMoons { get; private set; }
    public static ConfigEntry<bool> ShowNumberedPlanetNames { get; private set; }

    public static ConfigEntry<bool> ShowDecorations { get; private set; }
    public static ConfigEntry<bool> ShowGroupDividerLines { get; private set; }
    public static ConfigEntry<bool> ShowHelpText { get; private set; }

    private ConfigManager(ConfigFile config)
    {
      configFile = config;

      LoggingLevels = configFile.Bind("Debug", "Logging Levels", LoggingType.Basic, "Set the logging level for the mod");

      DetailedScanPage = configFile.Bind("General", "Detailed Scan Page", true, "Enable detailed scan page");

      UseShortenedWeathers = configFile.Bind("Moons", "Use Shortened Weathers", false, "Use shortened weathers in moons catalogue");

      ShowNumberedPlanetNames = configFile.Bind("Moons", "Show Numbered Planet Names", false, "Show numbered planet names in terminal");

      AlwaysDisplayHiddenMoons = configFile.Bind(
        "Moons",
        "Always Display Hidden Moons",
        false,
        "Always display hidden moons in moons catalogue"
      );

      ShowDecorations = configFile.Bind("General", "Show Decorations", false, "Show decorations in terminal");

      ShowGroupDividerLines = configFile.Bind("General", "Show Group Divider Lines", false, "Show group divider lines in terminal");

      ShowHelpText = configFile.Bind("General", "Show Help Text", false, "Show help text in terminal");

      DivideShopPage = configFile.Bind("Store", "Divide shop page into groups", 5, "Number of items per shop section (set to 0 to disable)");
    }
  }
}
