using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using TerminalFormatter.Compatibility;
using TerminalUtils.InfoTypes.Moons;
using UnityEngine;

namespace TerminalFormatter
{
  [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
  [BepInDependency(TerminalUtils.MyPluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.HardDependency)]
  public class Plugin : BaseUnityPlugin
  {
    internal static ManualLogSource logger;
    internal static Logger debugLogger = new("Debug", MrovLib.LoggingType.Developer);
    internal static Harmony harmony = new(MyPluginInfo.PLUGIN_GUID);

    internal static TerminalNode LockedNode;

    internal static DawnLibCompatibility DawnLibCompat;
    internal static LLLCompatibility LLLCompat;
    internal static WeatherRegistryCompatibility WeatherRegistryCompat;

    private void Awake()
    {
      logger = Logger;
      harmony.PatchAll();

      ConfigManager.Init(Config);

      DawnLibCompat = new DawnLibCompatibility("com.github.teamxiaolan.dawnlib");

      LLLCompat = new LLLCompatibility("imabatby.lethallevelloader");
      WeatherRegistryCompat = new WeatherRegistryCompatibility("mrov.WeatherRegistry");

      MrovLib.EventManager.ContentManagerReady.AddListener(() =>
      {
        new Nodes.Moons();

        new Nodes.Route();
        new Nodes.RouteAfter();
        new Nodes.RouteLocked();

        new Nodes.Scan();

        new Nodes.Store();

        new Nodes.Buy();
        new Nodes.BuyAfter();
        new Nodes.CannotAfford();

        new Nodes.Bestiary();

        new Nodes.Storage();

        Variables.ShipCache = null;
      });

      LockedNode = ScriptableObject.CreateInstance<TerminalNode>();
      LockedNode.name = "RouteLocked";
      LockedNode.clearPreviousText = true;
      LockedNode.acceptAnything = true;
      LockedNode.displayText = $"You cannot route to the selected moon. The route is locked.";
      LockedNode.terminalOptions = [];

      // Plugin startup logic
      Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
  }
}
