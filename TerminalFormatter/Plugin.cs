using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;

namespace TerminalFormatter
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    // [BepInDependency("imabatby.lethallevelloader", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(
        "Toskan4134.LethalRegeneration",
        BepInDependency.DependencyFlags.SoftDependency
    )]
    [BepInDependency(
        "com.potatoepet.AdvancedCompany",
        BepInDependency.DependencyFlags.SoftDependency
    )]
    [BepInDependency(
        "com.malco.lethalcompany.moreshipupgrades",
        BepInDependency.DependencyFlags.SoftDependency
    )]
    [BepInDependency("WeatherTweaks", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource logger;
        internal static Harmony harmony = new(PluginInfo.PLUGIN_GUID);

        internal static bool isACPresent = false;
        internal static bool isLLibPresent = false;
        internal static bool isLLLPresent = false;
        internal static bool isLRegenPresent = false;
        internal static bool isLGUPresent = false;
        internal static bool isWTPresent = false;

        private void Awake()
        {
            logger = Logger;
            harmony.PatchAll();

            ConfigManager.Init(Config);

            if (Chainloader.PluginInfos.ContainsKey("com.potatoepet.AdvancedCompany"))
            {
                logger.LogWarning("AC found, setting up compatibility patches");
                ACCompatibility.Init("com.potatoepet.AdvancedCompany");
                isACPresent = true;
            }

            if (Chainloader.PluginInfos.ContainsKey("imabatby.lethallevelloader"))
            {
                logger.LogWarning("LLL found, setting up compatibility patches");
                LLLCompatibility.Init();
                isLLLPresent = true;

                // new Nodes.Moons();
                // new Nodes.Simulate();
            }
            else
            {
                new Nodes.MoonsNoLLL();
            }

            if (Chainloader.PluginInfos.ContainsKey("evaisa.lethallib"))
            {
                logger.LogWarning("LethalLib found, setting up compatibility patches");
                // LLLCompatibility.Init();
                isLLibPresent = true;
            }

            if (Chainloader.PluginInfos.ContainsKey("Toskan4134.LethalRegeneration"))
            {
                logger.LogWarning("LethalRegeneration found, setting up compatibility patches");
                LethalRegenCompatibility.Init();
                isLRegenPresent = true;
            }

            if (Chainloader.PluginInfos.ContainsKey("com.malco.lethalcompany.moreshipupgrades"))
            {
                logger.LogWarning("MoreShipUpgrades found, setting up compatibility patches");
                LategameUpgradesCompatibility.Init();
                isLGUPresent = true;
            }

            if (Chainloader.PluginInfos.ContainsKey("WeatherTweaks"))
            {
                logger.LogWarning("WeatherTweaks found, setting up compatibility patches");
                WeatherTweaksCompatibility.Init();
                isWTPresent = true;
            }

            new Nodes.Route();
            new Nodes.Scan();
            new Nodes.Store();

            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}
