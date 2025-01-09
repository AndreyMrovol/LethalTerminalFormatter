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
      : base("Bestiary", ["0_Bestiary"]) { }

    public override string GetNodeText(TerminalNode node, Terminal terminal)
    {
      var header = new Header().CreateHeaderWithoutLines("BESTIARY");
      var adjustedTable = new StringBuilder();

      adjustedTable.Append(header);
      adjustedTable.Append("\n");

      List<int> scannedCreatures = terminal.scannedEnemyIDs;

      if (terminal.scannedEnemyIDs == null || terminal.scannedEnemyIDs.Count == 0)
      {
        adjustedTable.Append("NO DATA COLLECTED");
        adjustedTable.Append("\n");
        adjustedTable.Append("SCAN CREATURES TO UNLOCK THEIR DATA");
        return adjustedTable.ToString();
      }

      List<Creature> creatures = ContentManager.Creatures.Where(x => scannedCreatures.Contains(x.InfoNode.creatureFileID)).ToList();

      foreach (Creature creature in creatures)
      {
        adjustedTable.Append(creature.Name);
        adjustedTable.Append("\n");
      }

      return adjustedTable.ToString();
    }
  }
}
