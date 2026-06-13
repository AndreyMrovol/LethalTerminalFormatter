using System.Text;

namespace TerminalFormatter.Nodes
{
  public class CannotAfford : TerminalFormatterNode
  {
    public CannotAfford()
      : base("CannotAfford", ["CannotAfford"]) { }

    public override bool IsNodeValid(TerminalNode node)
    {
      node.clearPreviousText = true;
      return true;
    }

    public override string GetNodeText(TerminalNode node)
    {
      var table = new ConsoleTables.ConsoleTable("Title", "Things");

      var header = new Header().CreateHeaderWithoutLines("COMPANY STORE");
      var adjustedTable = new StringBuilder();

      table.AddRow("YOUR CREDITS:", $"${Variables.Terminal.groupCredits}");
      table.AddRow("TOTAL:", $"${Variables.Terminal.totalCostOfItems}");

      adjustedTable.Append(header);
      adjustedTable.Append("\n\n");
      adjustedTable.Append("You cannot afford this purchase!");
      adjustedTable.Append("\n\n");
      adjustedTable.Append(table.ToStringCustomDecoration());

      return adjustedTable.ToString();
    }
  }
}
