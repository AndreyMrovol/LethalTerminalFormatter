using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;

namespace TerminalFormatter
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource logger;
        internal static Harmony harmony = new(PluginInfo.PLUGIN_GUID);

        private void Awake()
        {
            logger = Logger;
            harmony.PatchAll();

            ConfigManager.Init(Config);

            if (Chainloader.PluginInfos.ContainsKey("com.potatoepet.AdvancedCompany"))
            {
                ACCompatibility.Init("com.potatoepet.AdvancedCompany");
            }
            else
            {
                logger.LogInfo("AdvancedCompany not found");
            }

            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}
