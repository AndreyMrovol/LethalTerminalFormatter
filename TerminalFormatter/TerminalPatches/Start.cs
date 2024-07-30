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
        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        public static void StartPostfix2(Terminal __instance)
        {
            Variables.Reset();

            Plugin.debugLogger.LogDebug("Terminal Start");
            Variables.Terminal = __instance;

            Settings.terminalFontSize = __instance.screenText.textComponent.fontSize;

            // if (Settings.firstUse)
            // {
            //     Plugin.debugLogger.LogDebug("First use: " + Settings.firstUse);
            //     Settings.firstUse = false;
            // }

            List<TerminalNode> Nodes = Resources.FindObjectsOfTypeAll<TerminalNode>().ToList();
            // Plugin.debugLogger.LogWarning($"Nodes count: {Nodes.Count}");
            Variables.Nodes = Nodes;

            List<SelectableLevel> levels = MrovLib.SharedMethods.GetGameLevels();

            for (int i = 0; i < levels.Count; i++)
            {
                SelectableLevel level = levels[i];

                // Plugin.debugLogger.LogDebug($"Level: {level.PlanetName}");

                List<TerminalNode> possibleNodes = Nodes
                    .Where(x => x.buyRerouteToMoon == i || x.displayPlanetInfo == i)
                    .Distinct()
                    .ToList();

                if (MrovLib.Plugin.LLL.IsModPresent && possibleNodes.Count > 2)
                {
                    List<TerminalNode> LLLNodes = MrovLib.SharedMethods.GetLevelTerminalNodes(
                        level
                    );

                    possibleNodes.RemoveAll(node => !LLLNodes.Contains(node));
                }

                // Plugin.debugLogger.LogDebug($"Possible nodes count: {possibleNodes.Count}");

                for (int j = 0; j < possibleNodes.Count; j++)
                {
                    Plugin.debugLogger.LogDebug($"Node: {possibleNodes[j]}");

                    if (possibleNodes[j] == null)
                    {
                        continue;
                    }
                }

                RelatedNodes relatedNodes = new RelatedNodes
                {
                    Node = possibleNodes
                        .Where(node => node.buyRerouteToMoon == -2)
                        .Distinct()
                        .ToList()
                        .FirstOrDefault(),
                    NodeConfirm = possibleNodes
                        .Where(node => node.buyRerouteToMoon != -2)
                        .Distinct()
                        .ToList()
                        .LastOrDefault()
                };

                Variables.Routes.Add(new Route(level, relatedNodes));
            }

            List<Item> buyableItems = __instance.buyableItemsList.ToList();

            buyableItems.ForEach(item =>
            {
                Plugin.debugLogger.LogDebug($"Item: {item.itemName}");

                // Plugin.debugLogger.LogDebug($"Item index: {buyableItems.IndexOf(item)}");
                // Plugin.debugLogger.LogDebug($"Is terminal null: {__instance == null}");

                List<TerminalNode> possibleNodes = Nodes
                    .Where(x => x.buyItemIndex == buyableItems.IndexOf(item))
                    .ToList();

                // Plugin.debugLogger.LogDebug($"Possible nodes count: {possibleNodes.Count}");

                for (int i = 0; i < possibleNodes.Count; i++)
                {
                    // Plugin.debugLogger.LogDebug($"Node: {possibleNodes[i]}");

                    if (possibleNodes[i] == null)
                    {
                        continue;
                    }
                }

                RelatedNodes relatedNodes =
                    new()
                    {
                        Node = possibleNodes
                            .Where(node =>
                                node.isConfirmationNode && !node.name.ToLower().Contains("mapper")
                            )
                            .Distinct()
                            .ToList()
                            .FirstOrDefault(),
                        NodeConfirm = possibleNodes
                            .Where(node =>
                                !node.isConfirmationNode && !node.name.ToLower().Contains("mapper")
                            )
                            .Distinct()
                            .ToList()
                            .LastOrDefault()
                    };

                Variables.Buyables.Add(new BuyableItem(__instance, relatedNodes));
            });

            List<UnlockableItem> unlockables =
                StartOfRound.Instance.unlockablesList.unlockables.ToList();

            for (int i = 0; i < unlockables.Count; i++)
            {
                UnlockableItem unlockable = unlockables[i];

                Plugin.debugLogger.LogDebug($"Unlockable: {unlockable.unlockableName}");

                if (unlockable.suitMaterial != null)
                {
                    continue;
                }

                List<TerminalNode> possibleNodes = Nodes
                    .Where(x => x.shipUnlockableID == unlockables.IndexOf(unlockable))
                    .Distinct()
                    .ToList();

                if (CheckPossibleNodeNull(possibleNodes))
                {
                    // Plugin.debugLogger.LogDebug("Possible nodes are null");
                    continue;
                }

                RelatedNodes relatedNodes = new RelatedNodes
                {
                    Node = possibleNodes
                        .Where(node => !node.buyUnlockable)
                        .Distinct()
                        .ToList()
                        .FirstOrDefault(),
                    NodeConfirm = possibleNodes
                        .Where(node => node.buyUnlockable)
                        .Distinct()
                        .ToList()
                        .LastOrDefault()
                };

                if (relatedNodes.Node == null || relatedNodes.NodeConfirm == null)
                {
                    continue;
                }

                if (unlockable.unlockableType == 1 && unlockable.alwaysInStock == true)
                {
                    // Plugin.debugLogger.LogDebug($"Unlockable, id{unlockables.IndexOf(unlockable)}");
                    Variables.Buyables.Add(new BuyableUnlockable(__instance, relatedNodes));
                }
                else
                {
                    // Plugin.debugLogger.LogDebug($"Decoration, id{unlockables.IndexOf(unlockable)}");
                    Variables.Buyables.Add(new BuyableDecoration(__instance, relatedNodes));
                }
            }

            List<BuyableVehicle> buyableVehicles = __instance.buyableVehicles.ToList();

            for (int i = 0; i < buyableVehicles.Count; i++)
            {
                BuyableVehicle vehicle = buyableVehicles[i];

                Plugin.debugLogger.LogDebug($"Vehicle: {vehicle.vehicleDisplayName}");

                List<TerminalNode> possibleNodes = Nodes
                    .Where(x => x.buyVehicleIndex == buyableVehicles.IndexOf(vehicle))
                    .Distinct()
                    .ToList();

                if (CheckPossibleNodeNull(possibleNodes))
                {
                    // Plugin.debugLogger.LogDebug("Possible nodes are null");
                    continue;
                }

                RelatedNodes relatedNodes = new RelatedNodes
                {
                    Node = possibleNodes
                        .Where(node => node.isConfirmationNode)
                        .Distinct()
                        .ToList()
                        .FirstOrDefault(),
                    NodeConfirm = possibleNodes
                        .Where(node => !node.isConfirmationNode)
                        .Distinct()
                        .ToList()
                        .LastOrDefault()
                };

                if (relatedNodes.Node == null || relatedNodes.NodeConfirm == null)
                {
                    continue;
                }

                Variables.Buyables.Add(new BuyableCar(__instance, relatedNodes));
            }
        }

        internal static bool CheckPossibleNodeNull(List<TerminalNode> possibleNodes)
        {
            List<TerminalNode> Nodes = [];

            for (int j = 0; j < possibleNodes.Count; j++)
            {
                Plugin.debugLogger.LogDebug($"Node: {possibleNodes[j]}");

                // somehow call continue on the upper loop

                if (possibleNodes[j] == null)
                {
                    continue;
                }

                if (possibleNodes[j].itemCost <= 0)
                {
                    continue;
                }

                // Plugin.debugLogger.LogDebug(
                //     $"Is null: {possibleNodes[j] == null}; {possibleNodes[j].itemCost <= 0}"
                // );

                Nodes.Add(possibleNodes[j]);
            }

            return possibleNodes == Nodes;
        }
    }
}
