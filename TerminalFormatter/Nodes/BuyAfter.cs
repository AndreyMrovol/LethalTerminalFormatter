using System.Linq;
using System.Text;
using MrovLib;
using MrovLib.ContentType;

namespace TerminalFormatter.Nodes
{
  public class BuyAfter : TerminalFormatterNode
  {
    public BuyAfter()
      : base("BuyAfter", ["Node2", "node2", "confirm", "2", "buy", "BuyNode", "Buy", "Buy1", "Node1"]) { }

    // Buy after: success/failure

    internal BuyableThing LastResolvedBuyable;

    public BuyableThing ResolveNodeIntoBuyable(TerminalNode node)
    {
      Plugin.logger.LogDebug($"Resolving node {node.name} into Buyable");

      return ContentManager
        .Buyables.Where(x => x.Nodes != null && x.Nodes.NodeConfirm != null)
        .Where(x => x.Nodes.NodeConfirm == node || x.Nodes.NodeConfirm.name == node.name)
        .FirstOrDefault();
    }

    public override bool IsNodeValid(TerminalNode node, Terminal terminal)
    {
      // check if that node is registered as NodeConfirm in Buyables

      BuyableThing resolvedItem = ResolveNodeIntoBuyable(node);

      return resolvedItem != null;
    }

    public override string GetNodeText(TerminalNode node, Terminal terminal)
    {
      var table = new ConsoleTables.ConsoleTable("Title", "Things");

      BuyableThing resolvedThing = ResolveNodeIntoBuyable(node);
      LastResolvedBuyable = resolvedThing;

      var header = new Header().CreateHeaderWithoutLines("SUCCESS!");
      var adjustedTable = new StringBuilder();

      table.AddRow("ITEM:", resolvedThing.Name);

      bool isItem = typeof(BuyableItem) == resolvedThing.GetType();
      int amount = terminal.playerDefinedAmount;

      adjustedTable.Append(header);
      adjustedTable.Append("\n\n");
      adjustedTable.Append("Thank you for your purchase!\n");

      if (isItem)
      {
        adjustedTable.Append(
          $"Your {(amount == 1 ? "item" : "items")} ({terminal.playerDefinedAmount} x {resolvedThing.Name}) {(amount == 1 ? "is" : "are")} on {(amount == 1 ? "its" : "their")} way!"
        );
      }
      else
      {
        adjustedTable.Append("Your unlockable is now available!");
      }

      return adjustedTable.ToString();
    }
  }
}
