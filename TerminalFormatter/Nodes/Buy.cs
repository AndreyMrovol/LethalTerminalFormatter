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
            : base("Buy", ["buy", "BuyNode", "Buy", "Buy1", "Node1"]) { }

        // Buy stuff confirmation
        // Buy after: success/failure

        // Route after: success/failure

        internal BuyableThing LastResolvedBuyable;

        public BuyableThing ResolveNodeIntoBuyable(TerminalNode node)
        {
            return Variables
                .Buyables.Where(x => x.Nodes.Node == node || x.Nodes.Node.name == node.name)
                .FirstOrDefault();
        }

        public override bool IsNodeValid(TerminalNode node, Terminal terminal)
        {
            // check if that node is registered as Node or NodeConfirm in Buyables

            BuyableThing resolvedItem = ResolveNodeIntoBuyable(node);

            if (resolvedItem != null)
            {
                Plugin.logger.LogWarning(
                    $"Resolved Item: {resolvedItem.Name}({resolvedItem.Type})"
                );
            }

            return resolvedItem != null;
        }

        public override string GetNodeText(TerminalNode node, Terminal terminal)
        {
            Plugin.logger.LogDebug("Patching BuyNode");

            var table = new ConsoleTables.ConsoleTable("Title", "Things");

            BuyableThing resolvedThing = ResolveNodeIntoBuyable(node);
            LastResolvedBuyable = resolvedThing;

            var header = new Header().CreateHeaderWithoutLines("CONFIRM PURCHASE");
            var adjustedTable = new StringBuilder();

            table.AddRow("ITEM:", resolvedThing.Name);

            // table.AddRow("AMOUNT:", terminal.playerDefinedAmount.ToString());

            bool isItem = typeof(BuyableItem) == resolvedThing.GetType();

            if (isItem)
            {
                table.AddRow("PRICE:", $"${resolvedThing.Price}");
            }

            if (node.buyItemIndex >= 0)
            {
                if (terminal.itemSalesPercentages[node.buyItemIndex] != 100)
                {
                    table.AddRow(
                        "DISCOUNT:",
                        $"{100 - terminal.itemSalesPercentages[node.buyItemIndex]}%"
                    );
                }
            }

            if (isItem)
            {
                table.AddRow("AMOUNT:", terminal.playerDefinedAmount.ToString());
            }

            table.AddRow("", "");

            table.AddRow(
                "TOTAL: ",
                $"${terminal.totalCostOfItems}  (${terminal.groupCredits - terminal.totalCostOfItems} after purchase)"
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
