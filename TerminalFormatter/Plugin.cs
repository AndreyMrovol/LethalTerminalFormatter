using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;

namespace TerminalFormatter
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("imabatby.lethallevelloader", BepInDependency.DependencyFlags.HardDependency)]
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
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource logger;
        internal static Harmony harmony = new(PluginInfo.PLUGIN_GUID);

        internal static bool isACPresent = false;
        internal static bool isLLLPresent = false;
        internal static bool isLRegenPresent = false;
        internal static bool isLGUPresent = false;

        private void Awake()
        {
            logger = Logger;
            harmony.PatchAll();

            ConfigManager.Init(Config);

            if (Chainloader.PluginInfos.ContainsKey("com.potatoepet.AdvancedCompany"))
            {
                ACCompatibility.Init("com.potatoepet.AdvancedCompany");
                isACPresent = true;
            }

            if (Chainloader.PluginInfos.ContainsKey("imabatby.lethallevelloader"))
            {
                LLLCompatibility.Init();
                isLLLPresent = true;
            }

            if (Chainloader.PluginInfos.ContainsKey("Toskan4134.LethalRegeneration"))
            {
                LethalRegenCompatibility.Init();
                isLRegenPresent = true;
            }

            if (Chainloader.PluginInfos.ContainsKey("com.malco.lethalcompany.moreshipupgrades"))
            {
                LategameUpgradesCompatibility.Init();
                isLGUPresent = true;
            }

            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}
