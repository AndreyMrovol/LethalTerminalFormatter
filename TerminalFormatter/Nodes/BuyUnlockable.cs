using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace TerminalFormatter.Nodes
{
    public class BuyUnlockable : TerminalFormatterNode
    {
        public BuyUnlockable()
            : base("BuyUnlockable", ["Buy", "buy", "Buy1", "Node1"]) { }

        // Buy stuff confirmation
        // Buy after: success/failure

        // Route after: success/failure

        public override bool IsNodeValid(TerminalNode node, Terminal terminal)
        {
            return node.shipUnlockableID > 0 && !node.name.ToLower().Contains("confirm");
        }

        public override string GetNodeText(TerminalNode node, Terminal terminal)
        {
            Plugin.logger.LogDebug("Patching UnlockableBuyNode");

            var table = new ConsoleTables.ConsoleTable("Title", "Things");

            Plugin.logger.LogInfo("Creating purchase table");

            var header = new Header().CreateHeaderWithoutLines("CONFIRM PURCHASE");
            var adjustedTable = new StringBuilder();

            UnlockableItem unlockable = StartOfRound.Instance.unlockablesList.unlockables[
                node.shipUnlockableID
            ];
            int price = node.itemCost;

            table.AddRow("ITEM:", unlockable.unlockableName);
            // table.AddRow("AMOUNT:", terminal.playerDefinedAmount.ToString());
            table.AddRow("PRICE:", $"${price}");

            table.AddRow("", "");
            table.AddRow(
                "TOTAL: ",
                $"${price * terminal.playerDefinedAmount} (${terminal.groupCredits - price * terminal.playerDefinedAmount} after purchase)"
            );

            adjustedTable.Append(header);
            adjustedTable.Append("\n\n");
            adjustedTable.Append("Please CONFIRM or DENY the purchase:");
            adjustedTable.Append("\n\n");
            adjustedTable.Append(table.ToStringCustomDecoration());

            return adjustedTable.ToString();
        }
    }
}
