using System.Collections.Generic;
using System.Linq;
using System.Text;
using MrovLib;
using MrovLib.ContentType;

namespace TerminalFormatter.Nodes
{
  public class Storage : TerminalFormatterNode
  {
    public Storage()
      : base("Storage", ["ItemsInStorage"]) { }

    public override string GetNodeText(TerminalNode node, Terminal terminal)
    {
      var header = new Header().CreateHeaderWithoutLines("STORAGE");
      var adjustedTable = new StringBuilder();

      adjustedTable.Append(header);
      adjustedTable.Append("\n");

      List<BuyableUnlockable> unlockablesInStorage = ContentManager.Unlockables.Where(unlockable => unlockable.IsInStorage).ToList();

      if (unlockablesInStorage.Count == 0)
      {
        adjustedTable.Append("No items stored.\n");
        adjustedTable.Append("To store items, use [X] while moving the object.");
        return adjustedTable.ToString();
      }

      foreach (BuyableUnlockable unlockable in unlockablesInStorage)
      {
        adjustedTable.Append(unlockable.Name);
        adjustedTable.Append("\n");
      }

      return adjustedTable.ToString();
    }
  }
}
