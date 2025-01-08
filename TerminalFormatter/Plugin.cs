using System.Linq;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace TerminalFormatter
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
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
    [BepInDependency("MrovLib", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource logger;
        internal static MrovLib.Logger debugLogger = new(PluginInfo.PLUGIN_GUID);
        internal static Harmony harmony = new(PluginInfo.PLUGIN_GUID);

        internal static TerminalNode LockedNode;

        internal static bool isACPresent = false;
        internal static bool isLLibPresent = false;
        internal static bool isLLLPresent = false;
        internal static bool isLRegenPresent = false;
        internal static bool isLGUPresent = false;
        internal static bool isWTPresent = false;
        internal static bool isLQPresent = false;

        internal static MrovLib.Compatibility.CompatibilityBase LGUCompat;
        internal static MrovLib.Compatibility.CompatibilityBase LQCompat;

        private void Awake()
        {
            logger = Logger;
            harmony.PatchAll();

            ConfigManager.Init(Config);

            MrovLib.EventManager.TerminalStart.AddListener(
                (Terminal terminal) => Variables.Terminal = terminal
            );

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

                new Nodes.Moons();
                new Nodes.RouteLocked();
                new Nodes.Simulate();
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

            LGUCompat = new LategameUpgradesCompatibility(
                "com.malco.lethalcompany.moreshipupgrades"
            );

            LQCompat = new LethalQuantitiesCompatibility("LethalQuantities");

            if (Chainloader.PluginInfos.ContainsKey("WeatherTweaks"))
            {
                logger.LogWarning("WeatherTweaks found, setting up compatibility patches");
                WeatherTweaksCompatibility.Init();
                isWTPresent = true;
            }

            new Nodes.Route();
            new Nodes.RouteAfter();

            new Nodes.Scan();
            new Nodes.Store();

            new Nodes.Buy();
            new Nodes.BuyAfter();
            new Nodes.CannotAfford();

            LockedNode = GameObject.Instantiate(
                new TerminalNode
                {
                    name = "RouteLocked",
                    clearPreviousText = true,
                    acceptAnything = true,
                    displayText = $"You cannot route to the selected moon. The route is locked.",
                    terminalOptions = []
                }
            );

            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}
