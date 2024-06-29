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
            : base("Store", ["0_StoreHub"])
        {
            this.AdditionalInfo =
                " Welcome to the Company store. \n Use words BUY and INFO on any item. \n Order items in bulk by typing a number.";
        }

        public override bool IsNodeValid(TerminalNode node, Terminal terminal)
        {
            return true;
        }

        public override string GetNodeText(TerminalNode node, Terminal terminal)
        {
            var table = new ConsoleTables.ConsoleTable("Name", "Price", "Owned");
            var adjustedTable = new StringBuilder();

            var ACServerConfiguration = Variables.IsACActive
                ? ACCompatibility.ServerConfiguration.GetValue(null)
                : null;

            GameObject ship = GameObject.Find("/Environment/HangarShip");
            var ItemsOnShip = ship.GetComponentsInChildren<GrabbableObject>().ToList();

            bool decor = ConfigManager.ShowDecorations.Value;

            string headerName = "COMPANY STORE";
            string storeHeader = new Header().CreateHeaderWithoutLines(headerName);
            // adjustedTable.Append(
            //     storeHeader
            //         .Replace("&", new string('─', terminalWidth - 6 - headerName.Length))
            //         .Replace("^", new string(' ', terminalWidth - 6 - headerName.Length))
            // );

            adjustedTable.Append(storeHeader);

            if (ConfigManager.ShowHelpText.Value)
            {
                adjustedTable.Append(
                    this.AdditionalInfo != null ? $"\n{this.AdditionalInfo}\n\n" : ""
                );
            }

            table.AddRow("[ITEMS]", "", "");
            if (decor)
            {
                table.AddRow($"{new string('-', Settings.dividerLength)}", "", "");
            }

            List<Item> sortedBuyableItemList = Variables
                .BuyableItemList.OrderBy(x => x.itemName)
                .ToList();

            int itemCount = 1;
            // every 3 items make a space

            // [buyableItemsList]
            foreach (var item in sortedBuyableItemList)
            {
                var index = terminal.buyableItemsList.ToList().IndexOf(item);
                var itemName = item.itemName;
                int howManyOnShip = ItemsOnShip
                    .FindAll(x => x.itemProperties.itemName == item.itemName)
                    .Count;

                if (index == -1)
                {
                    continue;
                }

                if (decor)
                {
                    itemName = $"* {itemName}";
                }

                if (Plugin.isLLibPresent)
                {
                    if (LethalLibCompatibility.IsLLItemDisabled(item))
                    {
                        continue;
                    }
                }

                if (ACCompatibility.Items.ContainsKey(itemName))
                {
                    if (!(bool)ACCompatibility.Items[itemName])
                    {
                        Plugin.logger.LogDebug($"Item {itemName} is disabled");
                        continue;
                    }
                }

                string discountPercent =
                    terminal.itemSalesPercentages[index] != 100
                        ? $" {(decor ? "(" : "")}-{100 - terminal.itemSalesPercentages[index]}%{(decor ? ")" : "")}"
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
                    $"{(howManyOnShip == 0 ? "" : $"×{howManyOnShip.ToString("D2")}")}"
                // $"{(terminal.itemSalesPercentages[index] != 100 ? 100 - terminal.itemSalesPercentages[index] : "")}"
                );

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
            if (decor)
            {
                table.AddRow($"{new string('-', Settings.dividerLength)}", "", "");
            }

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

                string unlockableName = unlockable.unlockableName;

                if (decor)
                {
                    unlockableName = $"* {unlockableName}";
                }

                if (Plugin.isLLibPresent)
                {
                    if (LethalLibCompatibility.IsLLUpgradeDisabled(unlockable))
                    {
                        continue;
                    }
                }

                if (unlockableNode == null)
                {
                    var index = StartOfRound
                        .Instance.unlockablesList.unlockables.ToList()
                        .IndexOf(unlockable);

                    // get all possible TerminalNode s
                    var allNodes = GameObject.FindObjectsOfType<TerminalNode>().ToList();
                    unlockableNode = allNodes.Find(x => x.shipUnlockableID == index);
                }

                if (isUnlocked)
                    continue;

                table.AddRow(
                    unlockableName.PadRight(Settings.itemNameWidth),
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
                    if (decor)
                    {
                        table.AddRow($"{new string('-', Settings.dividerLength)}", "", "");
                    }

                    table.AddRow(
                        "Natural Regeneration",
                        $"${LethalRegenCompatibility.GetCost()}",
                        ""
                    );
                }
            }

            table.AddRow("", "", "");
            table.AddRow("[DECORATIONS]", "", "");
            if (decor)
            {
                table.AddRow($"{new string('-', Settings.dividerLength)}", "", "");
            }

            // [unlockablesSelectionList]
            List<TerminalNode> DecorSelection = Variables
                .DecorationsList.OrderBy(x => x.creatureName)
                .ToList();

            itemCount = 1;

            foreach (var decoration in DecorSelection)
            {
                UnlockableItem unlockable = StartOfRound.Instance.unlockablesList.unlockables[
                    decoration.shipUnlockableID
                ];

                string decorationName = decoration.creatureName;

                if (decor)
                {
                    decorationName = $"* {decorationName}";
                }

                if (Plugin.isLLibPresent)
                {
                    if (LethalLibCompatibility.IsLLUpgradeDisabled(unlockable))
                    {
                        continue;
                    }
                }

                if (unlockable.hasBeenUnlockedByPlayer || unlockable.alreadyUnlocked)
                {
                    continue;
                }

                table.AddRow(decorationName, $"${decoration.itemCost}", "");

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
