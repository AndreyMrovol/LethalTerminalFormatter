using HarmonyLib;
using TerminalUtils.InfoTypes.Moons;
using UnityEngine;

namespace TerminalFormatter.Patches
{
  [HarmonyPatch(typeof(HangarShipDoor))]
  public static class HangarShipDoorStartPatch
  {
    [HarmonyPostfix]
    [HarmonyPatch("Start")]
    public static void PatchMethod()
    {
      Plugin.debugLogger.LogDebug("Applying HangarShipDoor Start Patch");

      TerminalUtils.TerminalManager.PreviewInfoTypes.Add("NumberedName", new PreviewNameNumbered());
    }
  }
}
