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
    public class CannotAfford : TerminalFormatterNode
    {
        public CannotAfford()
            : base("CannotAfford", ["CannotAfford"]) { }

        public override bool IsNodeValid(TerminalNode node, Terminal terminal)
        {
            node.clearPreviousText = true;
            return true;
        }

        public override string GetNodeText(TerminalNode node, Terminal terminal)
        {
            var table = new ConsoleTables.ConsoleTable("Title", "Things");

            var header = new Header().CreateHeaderWithoutLines("COMPANY STORE");
            var adjustedTable = new StringBuilder();

            table.AddRow("YOUR CREDITS:", $"${terminal.groupCredits}");
            table.AddRow("TOTAL:", $"${terminal.totalCostOfItems}");

            adjustedTable.Append(header);
            adjustedTable.Append("\n\n");
            adjustedTable.Append("You cannot afford this purchase!");
            adjustedTable.Append("\n\n");
            adjustedTable.Append(table.ToStringCustomDecoration());

            return adjustedTable.ToString();
        }
    }
}
