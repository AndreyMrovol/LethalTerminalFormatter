using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using LethalLib.Extras;
using TerminalFormatter.Nodes;
using UnityEngine;

namespace TerminalFormatter
{
    [HarmonyPatch(typeof(Terminal))]
    public class TerminalPatches
    {
        private static TerminalNode lastNode;

        [HarmonyPostfix]
        [HarmonyPatch("TextPostProcess")]
        public static void TextPostProcessPrefix(
            string modifiedDisplayText,
            TerminalNode node,
            Terminal __instance
        )
        {
            string newDisplayText = null;

            // check if node.name contains any of TerminalFormatterNode.terminalNode strings
            List<TerminalFormatterNode> possibleNodes = Settings
                .RegisteredNodes.Where(formatterNode =>
                    formatterNode.terminalNode.Any(y => node.name.Contains(y))
                )
                .ToList();

            if (possibleNodes != null)
            {
                // Plugin.logger.LogDebug($"Possible nodes count: {possibleNodes.Count}");
            }

            for (int i = 0; i < possibleNodes.Count; i++)
            {
                TerminalFormatterNode currentNode = possibleNodes[i];

                if (currentNode != null)
                {
                    bool shouldRun = currentNode.IsNodeValid(node, __instance);
                    if (!shouldRun)
                    {
                        Plugin.logger.LogDebug($"Node {currentNode.name} is not valid");
                        continue;
                    }

                    if (!currentNode.Enabled.Value)
                    {
                        Plugin.logger.LogDebug($"Node {currentNode.name} is not enabled");
                        continue;
                    }

                    newDisplayText = currentNode.GetNodeText(node, __instance);
                }
                else
                {
                    continue;
                }
            }

            if (newDisplayText != null)
            {
                StringBuilder builder = new StringBuilder();

                if (__instance.displayingPersistentImage)
                {
                    builder.Append("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
                }

                builder.Append("\n\n");
                builder.Append(newDisplayText);
                builder.Append($"\n{new string('-', Settings.dividerLength)}\n");

                // TODO replace all the logger shit with the MrovLib future abstracted solution
                // Plugin.logger.LogMessage("New display text:\n" + newDisplayText);

                __instance.screenText.text = builder.ToString();
                __instance.currentText = builder.ToString();

                __instance.textAdded = 0;
                Settings.firstUse = false;
            }

            return;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Terminal), "LoadNewNode")]
        [HarmonyPriority(Priority.HigherThanNormal)]
        public static bool CheckIfLocked(Terminal __instance, TerminalNode node)
        {
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
                Plugin.logger.LogDebug("Node is RouteLocked");
                return true;
            }

            Route level = TerminalFormatter
                .Variables.Routes.Where(x => x.Nodes.Node == node)
                .FirstOrDefault();

            if (level == null)
            {
                Plugin.logger.LogDebug("Level is null");
                return true;
            }

            if (level.Level == null)
            {
                return true;
            }

            bool isLocked = MrovLib.API.SharedMethods.IsMoonLockedLLL(level.Level);

            if (isLocked)
            {
                Plugin.logger.LogInfo("Node is locked!!");

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
                LLLCompatibility.Init();
            }

            node.clearPreviousText = true;
        }

        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        public static void StartPostfix2(Terminal __instance)
        {
            Variables.Buyables.Clear();
            Variables.Routes.Clear();
            Variables.Nodes.Clear();

            Plugin.logger.LogDebug("Terminal Start");
            Variables.Terminal = __instance;

            // TODO replace all the logger shit with the MrovLib future abstracted solution

            // if (Settings.firstUse)
            // {
            //     Plugin.logger.LogDebug("First use: " + Settings.firstUse);
            //     Settings.firstUse = false;
            // }

            List<TerminalNode> Nodes = Resources.FindObjectsOfTypeAll<TerminalNode>().ToList();
            // Plugin.logger.LogWarning($"Nodes count: {Nodes.Count}");
            Variables.Nodes = Nodes;

            List<SelectableLevel> levels = MrovLib.API.SharedMethods.GetGameLevels();

            for (int i = 0; i < levels.Count; i++)
            {
                SelectableLevel level = levels[i];

                // Plugin.logger.LogDebug($"Level: {level.PlanetName}");

                List<TerminalNode> possibleNodes = Nodes
                    .Where(x => x.buyRerouteToMoon == i || x.displayPlanetInfo == i)
                    .Distinct()
                    .ToList();

                if (MrovLib.Plugin.LLL.IsModPresent && possibleNodes.Count > 2)
                {
                    List<TerminalNode> LLLNodes = MrovLib.API.SharedMethods.GetLevelTerminalNodes(
                        level
                    );

                    possibleNodes.RemoveAll(node => !LLLNodes.Contains(node));
                }

                // Plugin.logger.LogDebug($"Possible nodes count: {possibleNodes.Count}");

                for (int j = 0; j < possibleNodes.Count; j++)
                {
                    // Plugin.logger.LogDebug($"Node: {possibleNodes[j]}");

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
                // Plugin.logger.LogDebug($"Item: {item.itemName}");

                // Plugin.logger.LogDebug($"Item index: {buyableItems.IndexOf(item)}");
                // Plugin.logger.LogDebug($"Is terminal null: {__instance == null}");

                List<TerminalNode> possibleNodes = Nodes
                    .Where(x => x.buyItemIndex == buyableItems.IndexOf(item))
                    .ToList();

                // Plugin.logger.LogDebug($"Possible nodes count: {possibleNodes.Count}");

                for (int i = 0; i < possibleNodes.Count; i++)
                {
                    // Plugin.logger.LogDebug($"Node: {possibleNodes[i]}");

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

                // Plugin.logger.LogDebug($"Unlockable: {unlockable.unlockableName}");

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
                    // Plugin.logger.LogDebug("Possible nodes are null");
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
                    // Plugin.logger.LogDebug($"Unlockable, id{unlockables.IndexOf(unlockable)}");
                    Variables.Buyables.Add(new BuyableUnlockable(__instance, relatedNodes));
                }
                else
                {
                    // Plugin.logger.LogDebug($"Decoration, id{unlockables.IndexOf(unlockable)}");
                    Variables.Buyables.Add(new BuyableDecoration(__instance, relatedNodes));
                }
            }
        }

        internal static bool CheckPossibleNodeNull(List<TerminalNode> possibleNodes)
        {
            List<TerminalNode> Nodes = [];

            for (int j = 0; j < possibleNodes.Count; j++)
            {
                // Plugin.logger.LogDebug($"Node: {possibleNodes[j]}");

                // somehow call continue on the upper loop

                if (possibleNodes[j] == null)
                {
                    continue;
                }

                if (possibleNodes[j].itemCost <= 0)
                {
                    continue;
                }

                // Plugin.logger.LogDebug(
                //     $"Is null: {possibleNodes[j] == null}; {possibleNodes[j].itemCost <= 0}"
                // );

                Nodes.Add(possibleNodes[j]);
            }

            return possibleNodes == Nodes;
        }
    }
}
