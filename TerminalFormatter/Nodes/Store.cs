using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HarmonyLib;
using UnityEngine;

namespace TerminalFormatter.Nodes
{
    public class Store : TerminalFormatterNode
    {
        public Store()
            : base("Store", ["0_StoreHub"]) { }

        public override bool IsNodeValid(TerminalNode node, Terminal terminal)
        {
            return true;
        }

        public override string GetNodeText(TerminalNode node, Terminal terminal)
        {
            var table = new ConsoleTables.ConsoleTable("Name", "Price", "# On ship");
            var adjustedTable = new StringBuilder();
            Plugin.logger.LogDebug("Patching 0_StoreHub");

            var ACServerConfiguration = Variables.IsACActive
                ? ACCompatibility.ServerConfiguration.GetValue(null)
                : null;

            GameObject ship = GameObject.Find("/Environment/HangarShip");
            var ItemsOnShip = ship.GetComponentsInChildren<GrabbableObject>().ToList();

            string headerName = "COMPANY STORE";
            string storeHeader = new Header().CreateHeaderWithoutLines(headerName, 4);
            // adjustedTable.Append(
            //     storeHeader
            //         .Replace("&", new string('─', terminalWidth - 6 - headerName.Length))
            //         .Replace("^", new string(' ', terminalWidth - 6 - headerName.Length))
            // );

            adjustedTable.Append(storeHeader);

            table.AddRow("[ITEMS]", "", "");

            List<Item> sortedBuyableItemList = Variables
                .BuyableItemList.OrderBy(x => x.itemName)
                .ToList();

            int itemCount = 1;
            // every 3 items make a space

            // [buyableItemsList]
            foreach (var item in sortedBuyableItemList)
            {
                Plugin.logger.LogDebug($"Item: {item.itemName}");
                var index = terminal.buyableItemsList.ToList().IndexOf(item);
                var itemName = item.itemName;
                int howManyOnShip = ItemsOnShip
                    .FindAll(x => x.itemProperties.itemName == item.itemName)
                    .Count;

                if (index == -1)
                {
                    continue;
                }

                if (Plugin.isLLibPresent)
                {
                    if (LethalLibCompatibility.IsLLItemDisabled(item))
                    {
                        Plugin.logger.LogDebug($"Item {itemName} is disabled in LethalLib");
                        continue;
                    }
                }

                if (ACCompatibility.Items.ContainsKey(itemName))
                {
                    // Plugin.logger.LogDebug($"Item {itemName} is in AC config");
                    if ((bool)ACCompatibility.Items[itemName])
                    {
                        // Plugin.logger.LogDebug($"Item {itemName} is enabled");
                    }
                    else
                    {
                        Plugin.logger.LogDebug($"Item {itemName} is disabled");
                        continue;
                    }
                }

                string discountPercent =
                    terminal.itemSalesPercentages[index] != 100
                        ? $"  -{100 - terminal.itemSalesPercentages[index]}%"
                        : "";

                // what i want to do:
                // itemName [some spaces] ... [discountPercent]
                // so the discountPercent is padded to the right

                // make itemName length = itemNameWidth
                if (itemName.Length + discountPercent.Length > Settings.itemNameWidth)
                {
                    itemName =
                        itemName.Substring(0, Settings.itemNameWidth - 4 - discountPercent.Length)
                        + "... "
                        + discountPercent;
                }
                else
                {
                    itemName =
                        $"{itemName.PadRight(Settings.itemNameWidth - discountPercent.Length)}{discountPercent}".PadRight(
                            Settings.itemNameWidth
                        );
                }

                table.AddRow(
                    itemName,
                    $"${item.creditsWorth * ((float)terminal.itemSalesPercentages[index] / 100f)}",
                    $"{(howManyOnShip == 0 ? "" : howManyOnShip.ToString())}"
                // $"{(terminal.itemSalesPercentages[index] != 100 ? 100 - terminal.itemSalesPercentages[index] : "")}"
                );

                Plugin.logger.LogDebug($"{itemCount}");

                if (ConfigManager.DivideShopPage.Value != 0)
                {
                    if (itemCount % ConfigManager.DivideShopPage.Value == 0)
                    {
                        itemCount = 1;
                        table.AddRow("", "", "");
                    }
                    else
                    {
                        itemCount++;
                    }
                }
            }

            table.AddRow("", "", "");
            table.AddRow("[UPGRADES]", "", "");

            Dictionary<string, int> upgrades =
                new()
                {
                    { "Teleporter", 375 },
                    { "Signal translator", 255 },
                    { "Loud horn", 100 },
                    { "Inverse Teleporter", 425 },
                };

            List<UnlockableItem> unlockablesList = Variables
                .UnlockableItemList.OrderBy(x => x.unlockableName)
                .ToList();

            foreach (var unlockable in unlockablesList)
            {
                bool isUnlocked = unlockable.hasBeenUnlockedByPlayer || unlockable.alreadyUnlocked;
                TerminalNode unlockableNode = unlockable.shopSelectionNode;

                if (Plugin.isLLibPresent)
                {
                    if (LethalLibCompatibility.IsLLUpgradeDisabled(unlockable))
                    {
                        Plugin.logger.LogDebug(
                            $"Upgrade {unlockable.unlockableName} is disabled in LethalLib"
                        );
                        continue;
                    }
                }

                if (unlockableNode == null)
                {
                    Plugin.logger.LogDebug(
                        $"UnlockableNode is null for {unlockable.unlockableName}"
                    );
                    var index = StartOfRound
                        .Instance.unlockablesList.unlockables.ToList()
                        .IndexOf(unlockable);

                    Plugin.logger.LogWarning(
                        $"Trying to find unlockableNode for {unlockable.unlockableName} with index {index}"
                    );

                    // get all possible TerminalNode s
                    var allNodes = GameObject.FindObjectsOfType<TerminalNode>().ToList();
                    Plugin.logger.LogWarning($"allNodes count: {allNodes.Count}");
                    unlockableNode = allNodes.Find(x => x.shipUnlockableID == index);

                    if (unlockableNode == null)
                    {
                        Plugin.logger.LogWarning(
                            $"UnlockableNode is still null for {unlockable.unlockableName}"
                        );
                    }
                }

                // Plugin.logger.LogDebug($"{unlockable.unlockableName} isUnlocked: {isUnlocked}");

                if (isUnlocked)
                    continue;

                table.AddRow(
                    unlockable.unlockableName.PadRight(Settings.itemNameWidth),
                    $"${(unlockableNode ? unlockableNode.itemCost : upgrades[unlockable.unlockableName])}",
                    ""
                );
            }

            if (Plugin.isLRegenPresent)
            {
                if (
                    !LethalRegenCompatibility.IsUpgradeBought()
                    && LethalRegenCompatibility.IsUpgradeInStore
                )
                {
                    table.AddRow("", "", "");
                    table.AddRow("[REGENERATION]", "", "");

                    table.AddRow(
                        "Natural Regeneration",
                        $"${LethalRegenCompatibility.GetCost()}",
                        ""
                    );
                }
            }

            table.AddRow("", "", "");
            table.AddRow("[DECORATIONS]", "", "");

            // [unlockablesSelectionList]
            List<TerminalNode> DecorSelection = Variables
                .DecorationsList.OrderBy(x => x.creatureName)
                .ToList();

            itemCount = 1;

            foreach (var decor in DecorSelection)
            {
                UnlockableItem unlockable = StartOfRound.Instance.unlockablesList.unlockables[
                    decor.shipUnlockableID
                ];

                if (Plugin.isLLibPresent)
                {
                    if (LethalLibCompatibility.IsLLUpgradeDisabled(unlockable))
                    {
                        Plugin.logger.LogDebug(
                            $"Upgrade {unlockable.unlockableName} is disabled in LethalLib"
                        );
                        continue;
                    }
                }

                Plugin.logger.LogDebug(
                    $"{decor.creatureName} isUnlocked: {unlockable.hasBeenUnlockedByPlayer} unlockable: {unlockable}"
                );
                if (unlockable.hasBeenUnlockedByPlayer || unlockable.alreadyUnlocked)
                {
                    continue;
                }

                table.AddRow(decor.creatureName, $"${decor.itemCost}", "");

                if (ConfigManager.DivideShopPage.Value != 0)
                {
                    if (itemCount % ConfigManager.DivideShopPage.Value == 0)
                    {
                        itemCount = 1;
                        table.AddRow("", "", "");
                    }
                    else
                    {
                        itemCount++;
                    }
                }
            }

            table.AddRow("", "", "");

            //

            string tableString = table.ToStringCustomDecoration(header: true, divider: true);

            // Regex replaceHorizontal = new(@"^\|-+\|-+\|-+\|\n");
            // Regex middleLineReplace = new(@"(?:\ |\-)\|(?:\ |\-)");
            // Regex pipeReplace = new(@"\|");

            // replaceHorizontal.Replace(tableString, "");

            // string modifiedTableString = middleLineReplace.Replace(tableString, "   ");
            // modifiedTableString = pipeReplace.Replace(modifiedTableString, "").Replace("-", " ");

            adjustedTable.Append(tableString);

            string finalString = adjustedTable.ToString().TrimEnd();
            return finalString;
        }
    }
}
