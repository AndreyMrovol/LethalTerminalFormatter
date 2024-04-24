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
    public class Buy : TerminalFormatterNode
    {
        public Buy()
            : base("Buy", ["buy", "BuyNode"]) { }

        // Buy stuff confirmation
        // Buy after: success/failure

        // Route after: success/failure

        public override bool IsNodeValid(TerminalNode node, Terminal terminal)
        {
            return node.isConfirmationNode && node.buyItemIndex > 0;
        }

        public override string GetNodeText(TerminalNode node, Terminal terminal)
        {
            Plugin.logger.LogDebug("Patching BuyNode");

            var table = new ConsoleTables.ConsoleTable("Title", "Things");

            Plugin.logger.LogInfo("Creating purchase table");

            var header = new Header().CreateHeaderWithoutLines("CONFIRM PURCHASE");
            var adjustedTable = new StringBuilder();

            Item item = terminal.buyableItemsList[node.buyItemIndex];
            int price = item.creditsWorth;

            table.AddRow("ITEM:", item.itemName);
            table.AddRow("AMOUNT:", terminal.playerDefinedAmount.ToString());
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
