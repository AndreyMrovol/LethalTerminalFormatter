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

        public override bool IsNodeValid(TerminalNode node, Terminal terminal)
        {
            // check if that node is registered as Node or NodeConfirm in Buyables

            BuyableThing resolvedItem = Variables
                .Buyables.Where(x => x.Nodes.Node == node)
                .ToList()
                .FirstOrDefault();

            return resolvedItem != null;
        }

        public override string GetNodeText(TerminalNode node, Terminal terminal)
        {
            Plugin.logger.LogDebug("Patching BuyNode");

            var table = new ConsoleTables.ConsoleTable("Title", "Things");

            BuyableThing resolvedThing = Variables
                .Buyables.Where(x => x.Nodes.Node == node)
                .FirstOrDefault();

            var header = new Header().CreateHeaderWithoutLines("CONFIRM PURCHASE");
            var adjustedTable = new StringBuilder();

            table.AddRow("ITEM:", resolvedThing.Name);

            if (typeof(BuyableItem) == resolvedThing.GetType())
            {
                table.AddRow("AMOUNT:", terminal.playerDefinedAmount.ToString());
            }

            // table.AddRow("AMOUNT:", terminal.playerDefinedAmount.ToString());
            table.AddRow("PRICE:", $"${resolvedThing.Price}");

            table.AddRow("", "");

            table.AddRow(
                "TOTAL: ",
                $"${resolvedThing.Price * terminal.playerDefinedAmount} (${terminal.groupCredits - resolvedThing.Price * terminal.playerDefinedAmount} after purchase)"
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
