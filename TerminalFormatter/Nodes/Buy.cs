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

            Plugin.logger.LogWarning($"Resolved Item: {resolvedItem}");

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

            int price = resolvedThing.Price;
            bool isDiscounted = terminal.itemSalesPercentages[node.buyItemIndex] != 100;

            // table.AddRow("AMOUNT:", terminal.playerDefinedAmount.ToString());
            table.AddRow("PRICE:", $"${resolvedThing.Price}");

            if (terminal.itemSalesPercentages[node.buyItemIndex] != 100)
            {
                table.AddRow(
                    "DISCOUNT:",
                    $"{100 - terminal.itemSalesPercentages[node.buyItemIndex]}%"
                );
            }

            if (typeof(BuyableItem) == resolvedThing.GetType())
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
