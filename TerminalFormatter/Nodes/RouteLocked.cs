using System.Text;

namespace TerminalFormatter.Nodes
{
    public class RouteLocked : TerminalFormatterNode
    {
        public RouteLocked()
            : base("RouteLocked", ["RouteLocked"]) { }

        public override bool IsNodeValid(TerminalNode node, Terminal terminal)
        {
            return true;
        }

        public override string GetNodeText(TerminalNode node, Terminal terminal)
        {
            var table = new ConsoleTables.ConsoleTable("Title", "Things");

            var header = new Header().CreateHeaderWithoutLines("ROUTE LOCKED");
            var adjustedTable = new StringBuilder();

            adjustedTable.Append(header);
            adjustedTable.Append("\n\n");
            adjustedTable.Append($"You cannot currently route to the selected moon.");
            adjustedTable.Append("\n\n");
            adjustedTable.Append(table.ToStringCustomDecoration());

            return adjustedTable.ToString();
        }
    }
}
