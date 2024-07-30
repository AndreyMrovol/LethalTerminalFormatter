using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using LethalLib.Extras;
using MrovLib;
using TerminalFormatter.Nodes;
using UnityEngine;

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

            if (!MrovLib.Plugin.LLL.IsModPresent)
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

            Route level = TerminalFormatter
                .Variables.Routes.Where(x => x.Nodes.Node == node)
                .FirstOrDefault();

            if (level == null)
            {
                Plugin.debugLogger.LogDebug("Level is null");
                return true;
            }

            if (level.Level == null)
            {
                return true;
            }

            bool isLocked = MrovLib.SharedMethods.IsMoonLockedLLL(level.Level);

            if (isLocked)
            {
                Plugin.debugLogger.LogInfo("Node is locked!!");

                __instance.LoadNewNode(Plugin.LockedNode);

                return false;
            }
            else
            {
                return true;
            }
        }

        [HarmonyPostfix]
        // [HarmonyPriority(Priority.Last)]
        [HarmonyAfter("imabatby.lethallevelloader")]
        [HarmonyPatch("LoadNewNode")]
        public static void StartPostfix(Terminal __instance, ref TerminalNode node)
        {
            Variables.BuyableItemList = __instance.buyableItemsList.ToList();
            Variables.UnlockableItemList = StartOfRound
                .Instance.unlockablesList.unlockables.Where(x =>
                    x.unlockableType == 1 && x.alwaysInStock == true
                )
                .ToList();
            Variables.DecorationsList = __instance.ShipDecorSelection;

            if (Variables.IsACActive)
            {
                ACCompatibility.Refresh();
            }

            if (Settings.firstUse && Variables.ISLLLActive)
            {
                LLLCompatibility.GetLLLSettings();
            }

            node.clearPreviousText = true;
        }
    }
}
