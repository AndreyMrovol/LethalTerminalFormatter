using System.Linq;
using System.Text;
using HarmonyLib;

namespace TerminalFormatter
{
    [HarmonyPatch(typeof(Terminal))]
    public class TerminalPatches
    {
        // [HarmonyPrefix]
        // [HarmonyPatch(nameof(Terminal.ParsePlayerSentence))]
        // static void CustomParser(ref Terminal __instance, ref TerminalNode __result)
        // {
        //     string text = __instance.screenText.text.Substring(
        //         __instance.screenText.text.Length - __instance.textAdded
        //     );
        //     CommandParser.ParseCommand(text, ref __instance, ref __result);
        // }

        [HarmonyPostfix]
        [HarmonyPatch("TextPostProcess")]
        public static void TextPostProcessPrefix(
            string modifiedDisplayText,
            TerminalNode node,
            Terminal __instance
        )
        {
            Plugin.logger.LogDebug(node.name);

            string newDisplayText = null;

            // Settings.RegisteredNodes.ForEach(terminalnode =>
            // {
            //     Plugin.logger.LogDebug($"Node: {terminalnode.name}");
            //     terminalnode.terminalNode.ForEach(terminalNode =>
            //     {
            //         Plugin.logger.LogDebug(
            //             $"{terminalNode}, contains: {node.name.Contains(terminalNode)} {terminalNode.Contains(node.name)}"
            //         );
            //     });
            // });

            // check if node.name contains any of TerminalFormatterNode.terminalNode strings
            TerminalFormatterNode currentNode = Settings
                .RegisteredNodes.Where(formatterNode =>
                    formatterNode.terminalNode.Any(y => node.name.Contains(y))
                )
                .FirstOrDefault();

            if (currentNode != null)
            {
                bool shouldRun = currentNode.IsNodeValid(node, __instance);
                if (!shouldRun)
                {
                    return;
                }

                if (!currentNode.Enabled.Value)
                {
                    return;
                }

                Plugin.logger.LogWarning($"Found node: {currentNode.name}");

                newDisplayText = currentNode.GetNodeText(node, __instance);
            }
            else
            {
                return;
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
                builder.Append("\n--------------------\n");

                Plugin.logger.LogMessage("New display text:\n" + newDisplayText);

                __instance.screenText.text = builder.ToString();
                __instance.currentText = builder.ToString();
                __instance.textAdded = 0;
            }

            return;
        }

        [HarmonyPostfix]
        // [HarmonyPriority(Priority.Last)]
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

            Plugin.logger.LogDebug("First use: " + Settings.firstUse);

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
            Plugin.logger.LogDebug("Terminal Start");

            // if (Settings.firstUse)
            // {
            //     Plugin.logger.LogDebug("First use: " + Settings.firstUse);
            //     Settings.firstUse = false;
            // }

            List<TerminalNode> Nodes = Resources.FindObjectsOfTypeAll<TerminalNode>().ToList();
            Plugin.logger.LogWarning($"Nodes count: {Nodes.Count}");
            Variables.Nodes = Nodes;

            List<Item> buyableItems = __instance.buyableItemsList.ToList();

            buyableItems.ForEach(item =>
            {
                Plugin.logger.LogDebug($"Item: {item.itemName}");

                Plugin.logger.LogDebug($"Item index: {buyableItems.IndexOf(item)}");
                Plugin.logger.LogDebug($"Is terminal null: {__instance == null}");

                List<TerminalNode> possibleNodes = Nodes
                    .Where(x => x.buyItemIndex == buyableItems.IndexOf(item))
                    .ToList();

                Plugin.logger.LogDebug($"Possible nodes count: {possibleNodes.Count}");

                for (int i = 0; i < possibleNodes.Count; i++)
                {
                    Plugin.logger.LogDebug($"Node: {possibleNodes[i]}");

                    if (possibleNodes[i] == null)
                    {
                        return;
                    }
                }

                RelatedNodes relatedNodes = new RelatedNodes
                {
                    Node = possibleNodes
                        .Where(node => node.isConfirmationNode)
                        .ToList()
                        .FirstOrDefault(),
                    NodeConfirm = possibleNodes
                        .Where(node => !node.isConfirmationNode)
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

                Plugin.logger.LogDebug($"Unlockable: {unlockable.unlockableName}");

                if (unlockable.suitMaterial != null)
                {
                    continue;
                }

                List<TerminalNode> possibleNodes = Nodes
                    .Where(x => x.shipUnlockableID == unlockables.IndexOf(unlockable))
                    .ToList();

                for (int j = 0; j < possibleNodes.Count; j++)
                {
                    Plugin.logger.LogDebug($"Node: {possibleNodes[j]}");

                    if (possibleNodes[j] == null)
                    {
                        return;
                    }

                    if (possibleNodes[j].itemCost <= 0)
                    {
                        return;
                    }
                }

                RelatedNodes relatedNodes = new RelatedNodes
                {
                    Node = possibleNodes
                        .Where(node => !node.buyUnlockable)
                        .ToList()
                        .FirstOrDefault(),
                    NodeConfirm = possibleNodes
                        .Where(node => node.buyUnlockable)
                        .ToList()
                        .LastOrDefault()
                };

                if (unlockable.unlockableType == 1 && unlockable.alwaysInStock == true)
                {
                    Plugin.logger.LogDebug($"Unlockable, id{unlockables.IndexOf(unlockable)}");
                    Variables.Buyables.Add(new BuyableUnlockable(__instance, relatedNodes));
                }
                else
                {
                    Plugin.logger.LogDebug($"Decoration, id{unlockables.IndexOf(unlockable)}");
                    Variables.Buyables.Add(new BuyableDecoration(__instance, relatedNodes));
                }
            }
            ;
        }
    }
}
