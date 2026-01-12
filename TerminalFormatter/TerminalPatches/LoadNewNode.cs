using System.Linq;
using HarmonyLib;
using MrovLib.ContentType;

namespace TerminalFormatter
{
  [HarmonyPatch(typeof(Terminal))]
  public partial class TerminalPatches
  {
    private static TerminalNode lastNode;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Terminal), "LoadNewNode")]
    [HarmonyPriority(Priority.HigherThanNormal)]
    public static bool CheckIfLocked(Terminal __instance, TerminalNode node)
    {
      if (node == null)
      {
        return true;
      }

      lastNode = node;

      if (!MrovLib.Plugin.LLL.IsModPresent && !Plugin.DawnLibCompat.IsModPresent)
      {
        return true;
      }

      if (!node.name.ToLower().Contains("route") && !node.buyRerouteToMoon.Equals(-2))
      {
        return true;
      }

      if (node.name == "RouteLocked")
      {
        Plugin.debugLogger.LogDebug("Node is RouteLocked");
        return true;
      }

      Route level = MrovLib.ContentManager.Routes.FirstOrDefault(x => x.Nodes.Node == node);
      if (level == null)
      {
        Plugin.debugLogger.LogDebug("Level is null");
        return true;
      }

      if (level.Level == null)
      {
        return true;
      }

      if (node.displayText.ToLower().Contains("route locked!"))
      {
        if (Plugin.DawnLibCompat.IsModPresent)
        {
          if (Plugin.DawnLibCompat.GetLevelStatus(level.Level).locked)
          {
            Plugin.debugLogger.LogInfo("Node is locked by DawnLib!!");
            __instance.LoadNewNode(Plugin.LockedNode);
            return false;
          }
          else
          {
            return true;
          }
        }
      }

      if (MrovLib.SharedMethods.IsMoonLockedLLL(level.Level))
      {
        Plugin.debugLogger.LogInfo("Node is locked!!");
        __instance.LoadNewNode(Plugin.LockedNode);
        return false;
      }

      return true;
    }
  }
}
