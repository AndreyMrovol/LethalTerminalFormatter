using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using TerminalFormatter.Compatibility;
using UnityEngine;

namespace TerminalFormatter
{
  [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
  [BepInDependency("MrovLib", BepInDependency.DependencyFlags.HardDependency)]
  public class Plugin : BaseUnityPlugin
  {
    internal static ManualLogSource logger;
    internal static Logger debugLogger = new("Debug", MrovLib.LoggingType.Developer);
    internal static Harmony harmony = new(MyPluginInfo.PLUGIN_GUID);

    internal static TerminalNode LockedNode;

    internal static LethalLibCompatibility LethalLibCompat;
    internal static LategameUpgradesCompatibility LGUCompat;
    internal static StoreRotationConfigCompatibility SRCCompat;
    internal static LethalRegenCompatibility LethalRegenCompat;

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

      LethalLibCompat = new("evaisa.lethallib");

      LethalRegenCompat = new LethalRegenCompatibility("Toskan4134.LethalRegeneration");

      LGUCompat = new LategameUpgradesCompatibility("com.malco.lethalcompany.moreshipupgrades");
      SRCCompat = new StoreRotationConfigCompatibility("pacoito.StoreRotationConfig");

      DawnLibCompat = new DawnLibCompatibility("com.github.teamxiaolan.dawnlib");

      LLLCompat = new LLLCompatibility("imabatby.lethallevelloader");

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
      Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void MainMenuInit()
    {
      if (LLLCompat.IsModPresent)
      {
        new Nodes.Moons();
        new Nodes.RouteLocked();
        new Nodes.Simulate();
      }
      else
      {
        new Nodes.MoonsNoLLL();
      }
    }
  }
}
