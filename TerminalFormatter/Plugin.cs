using System.Collections.Generic;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using TerminalFormatter.Compatibility;
using UnityEngine;

namespace TerminalFormatter
{
  [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
  [BepInDependency("Toskan4134.LethalRegeneration", BepInDependency.DependencyFlags.SoftDependency)]
  [BepInDependency("pacoito.StoreRotationConfig", BepInDependency.DependencyFlags.SoftDependency)]
  [BepInDependency("WeatherTweaks", BepInDependency.DependencyFlags.SoftDependency)]
  [BepInDependency("MrovLib", BepInDependency.DependencyFlags.HardDependency)]
  public class Plugin : BaseUnityPlugin
  {
    internal static ManualLogSource logger;
    internal static Logger debugLogger = new("Debug", MrovLib.LoggingType.Developer);
    internal static Harmony harmony = new(PluginInfo.PLUGIN_GUID);

    internal static TerminalNode LockedNode;

    internal static bool isLLibPresent = false;
    internal static bool isLLLPresent
    {
      get { return LLLCompat.IsModPresent; }
    }
    internal static bool isLRegenPresent = false;

    internal static bool isLGUPresent
    {
      get { return LGUCompat.IsModPresent; }
    }
    internal static bool isWTPresent = false;
    internal static bool isLQPresent
    {
      get { return LQCompat.IsModPresent; }
    }
    internal static bool isSRCPresent
    {
      get { return SRCCompat.IsModPresent; }
    }

    internal static MrovLib.Compatibility.CompatibilityBase LGUCompat;
    internal static MrovLib.Compatibility.CompatibilityBase LQCompat;
    internal static MrovLib.Compatibility.CompatibilityBase SRCCompat;
    internal static DawnLibCompatibility DawnLibCompat;
    internal static LLLCompatibility LLLCompat;

    private void Awake()
    {
      logger = Logger;
      harmony.PatchAll();

      ConfigManager.Init(Config);

      MrovLib.EventManager.TerminalStart.AddListener((Terminal terminal) => Variables.Terminal = terminal);

      MrovLib.EventManager.MainMenuLoaded.AddListener(() =>
      {
        MainMenuInit();
      });

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

      LGUCompat = new LategameUpgradesCompatibility("com.malco.lethalcompany.moreshipupgrades");

      LQCompat = new LethalQuantitiesCompatibility("LethalQuantities");

      SRCCompat = new StoreRotationConfigCompatibility("pacoito.StoreRotationConfig");

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

      new Nodes.Bestiary();

      new Nodes.Storage();

      LockedNode = ScriptableObject.CreateInstance<TerminalNode>();
      LockedNode.name = "RouteLocked";
      LockedNode.clearPreviousText = true;
      LockedNode.acceptAnything = true;
      LockedNode.displayText = $"You cannot route to the selected moon. The route is locked.";
      LockedNode.terminalOptions = [];

      // Plugin startup logic
      Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void MainMenuInit()
    {
      DawnLibCompat = new DawnLibCompatibility("com.github.teamxiaolan.dawnlib");
      if (DawnLibCompat.IsModPresent)
      {
        logger.LogWarning("DawnLib found, setting up compatibility patches");
        DawnLibCompat.Init();
      }

      LLLCompat = new LLLCompatibility("imabatby.lethallevelloader");
      if (LLLCompat.IsModPresent)
      {
        Plugin.LLLCompat.Init();

        new Nodes.Moons();
        new Nodes.RouteLocked();
        new Nodes.Simulate();
      }
      else
      {
        new Nodes.MoonsNoLLL();
      }

      LGUCompat = new LategameUpgradesCompatibility("com.malco.lethalcompany.moreshipupgrades");
    }
  }
}
