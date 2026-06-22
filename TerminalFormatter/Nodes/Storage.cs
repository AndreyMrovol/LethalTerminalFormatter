using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerminalFormatter.Nodes
{
  public class Storage : TerminalFormatterNode
  {
    public Storage()
      : base("Storage", ["ItemsInStorage"])
    {
      this.HelpText = " Welcome to the Storage! \n Type item's name to retrieve it. \n To store items, use [X] while moving the object.";
    }

    public override string GetNodeText(TerminalNode node)
    {
      var header = new Header().CreateHeaderWithoutLines("STORAGE");
      var adjustedTable = new StringBuilder();

      adjustedTable.Append(header);

      if (ConfigManager.ShowHelpText.Value)
      {
        adjustedTable.Append(this.HelpText != null ? $"\n{this.HelpText}\n\n" : "");
      }
      else
      {
        adjustedTable.Append("\n");
      }

      List<UnlockableItem> unlockablesInStorage = StartOfRound
        .Instance.unlockablesList.unlockables.Where(unlockable => unlockable.inStorage)
        .ToList();

      if (unlockablesInStorage.Count == 0)
      {
        adjustedTable.Append("No items stored.\n");
        return adjustedTable.ToString();
      }

      foreach (UnlockableItem unlockable in unlockablesInStorage)
      {
        if (ConfigManager.ShowDecorations.Value)
        {
          adjustedTable.Append(" * ");
        }
        else
        {
          adjustedTable.Append(" ");
        }

        adjustedTable.Append(unlockable.unlockableName);
        adjustedTable.Append("\n");
      }

      return adjustedTable.ToString();
    }
  }
}
