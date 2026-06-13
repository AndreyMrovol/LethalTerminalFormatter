using System.Collections.Generic;
using System.Linq;
using System.Text;
using MrovLib;
using MrovLib.ContentType;

namespace TerminalFormatter.Nodes
{
  public class Bestiary : TerminalFormatterNode
  {
    public Bestiary()
      : base("Bestiary", ["0_Bestiary"])
    {
      this.HelpText = " Welcome to the Bestiary! \n Use INFO after creature name to learn more.";
    }

    public override string GetNodeText(TerminalNode node)
    {
      var header = new Header().CreateHeaderWithoutLines("BESTIARY");
      var adjustedTable = new StringBuilder();

      adjustedTable.Append(header);

      List<int> scannedCreatures = Variables.Terminal.scannedEnemyIDs;

      if (ConfigManager.ShowHelpText.Value)
      {
        adjustedTable.Append(this.HelpText != null ? $"\n{this.HelpText}\n\n" : "");
      }
      else
      {
        adjustedTable.Append("\n");
      }

      // no scanned creatures yet
      if (Variables.Terminal.scannedEnemyIDs == null || Variables.Terminal.scannedEnemyIDs.Count == 0)
      {
        adjustedTable.Append(" NO DATA COLLECTED");
        adjustedTable.Append(" \n");
        adjustedTable.Append(" Scan creatures to unlock their data.");
        return adjustedTable.ToString();
      }

      List<Creature> creatures = ContentManager.Creatures.Where(x => scannedCreatures.Contains(x.InfoNode.creatureFileID)).ToList();

      foreach (Creature creature in creatures)
      {
        if (ConfigManager.ShowDecorations.Value)
        {
          adjustedTable.Append(" * ");
        }
        else
        {
          adjustedTable.Append(" ");
        }

        adjustedTable.Append(creature.Name);
        adjustedTable.Append("\n");
      }

      return adjustedTable.ToString();
    }
  }
}
