using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace TerminalFormatter
{
    partial class Nodes
    {
        // Store node
        private static readonly int itemNameWidth = TerminalPatches.terminalWidth - 9 - 10;

        public string Store(TerminalNode node, Terminal terminal)
        {
            var table = new ConsoleTables.ConsoleTable("Name", "Price", "# On ship");
            var adjustedTable = new StringBuilder();
            Plugin.logger.LogDebug("Patching 0_StoreHub");

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

            // [buyableItemsList]
            for (int index = 0; index < terminal.buyableItemsList.Length; ++index)
            {
                var item = terminal.buyableItemsList[index];
                var itemName = item.itemName;
                int howManyOnShip = ItemsOnShip
                    .FindAll(x => x.itemProperties.itemName == item.itemName)
                    .Count;

                string discountPercent =
                    terminal.itemSalesPercentages[index] != 100
                        ? $"  -{100 - terminal.itemSalesPercentages[index]}%"
                        : "";

                // make itemName length = itemNameWidth
                if (itemName.Length + discountPercent.Length > itemNameWidth)
                {
                    itemName =
                        itemName.Substring(0, itemNameWidth - 3 - discountPercent.Length)
                        + discountPercent
                        + "...";
                }
                else
                {
                    itemName = $"{itemName}{discountPercent}".PadRight(itemNameWidth);
                }

                table.AddRow(
                    itemName,
                    $"${item.creditsWorth * ((float)terminal.itemSalesPercentages[index] / 100f)}",
                    $"{(howManyOnShip == 0 ? "" : howManyOnShip.ToString())}"
                // $"{(terminal.itemSalesPercentages[index] != 100 ? 100 - terminal.itemSalesPercentages[index] : "")}"
                );
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

            foreach (var upgrade in upgrades)
            List<UnlockableItem> unlockablesList = Variables
                .UnlockableItemList.OrderBy(x => x.unlockableName)
                .ToList();
            {
                UnlockableItem unlockable = StartOfRound.Instance.unlockablesList.unlockables.Find(
                    unlockable => unlockable.unlockableName == upgrade.Key
                );
                bool isUnlocked = unlockable.hasBeenUnlockedByPlayer || unlockable.alreadyUnlocked;

                Plugin.logger.LogDebug(
                    $"{upgrade} isUnlocked: {isUnlocked} unlockable: {unlockable}"
                );

                if (isUnlocked)
                    continue;

                table.AddRow(upgrade.Key.PadRight(itemNameWidth), $"${upgrade.Value}", "");
            }

            table.AddRow("", "", "");
            table.AddRow("[DECORATIONS]", "", "");

            // [unlockablesSelectionList]
            List<TerminalNode> DecorSelection = Variables
                .DecorationsList.OrderBy(x => x.creatureName)
                .ToList();

            foreach (var decor in DecorSelection)
            {
                UnlockableItem unlockable = StartOfRound.Instance.unlockablesList.unlockables[
                    decor.shipUnlockableID
                ];

                Plugin.logger.LogDebug(
                    $"{decor.creatureName} isUnlocked: {unlockable.hasBeenUnlockedByPlayer} unlockable: {unlockable}"
                );
                if (unlockable.hasBeenUnlockedByPlayer || unlockable.alreadyUnlocked)
                {
                    continue;
                }

                table.AddRow(decor.creatureName, $"${decor.itemCost}", "");
            }

            table.AddRow("", "", "");

            //

            string tableString = RemoveTable(table.ToMarkDownString());

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
