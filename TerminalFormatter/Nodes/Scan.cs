using System.Collections.Generic;
using System.Linq;
using System.Text;
using LethalLevelLoader;
using MrovLib;
using UnityEngine;

namespace TerminalFormatter.Nodes
{
  public class Scan : TerminalFormatterNode
  {
    public Scan()
      : base("Scan", ["ScanInfo"]) { }

    public override bool IsNodeValid(TerminalNode node)
    {
      return true;
    }

    public override string GetNodeText(TerminalNode node)
    {
      TerminalUtils.Nodes.Scan utilsScanNode = (TerminalUtils.Nodes.Scan)
        TerminalUtils.TerminalManager.NodeReplacements[TerminalUtils.TerminalManager.ScanPage];

      bool detailedScan = ConfigManager.DetailedScanPage.Value;

      bool isShip = StartOfRound.Instance.inShipPhase;
      bool isCompanyMoon = LevelHelper.CompanyMoons.Contains(StartOfRound.Instance.currentLevel);

      var adjustedTable = new StringBuilder();
      adjustedTable.Append("\n");

      List<GrabbableObject> objectsToScan = utilsScanNode.GetObjects();
      int items = objectsToScan.Count;
      int value = objectsToScan.Sum(x => x.scrapValue);

      string headerName = "SCANNER";
      Dictionary<int, string> headerInfo =
        new()
        {
          { 0, $"SCANNING {(isShip ? "SHIP" : "MOON")}" },
          { 1, $"ITEMS: {items.ToString()}" },
          { 2, $"VALUE: ${value.ToString()}" },
        };
      string moonsHeader = new Header().CreateNumberedHeader(headerName, 2, headerInfo);
      adjustedTable.Append(moonsHeader);

      var table = new ConsoleTables.ConsoleTable("Name", "Price", "Two-handed?");

      foreach (GrabbableObject obj in objectsToScan)
      {
        table.AddRow(obj.itemProperties.itemName, $"${obj.scrapValue}", obj.itemProperties.twoHanded ? "●" : "○");
      }

      if (detailedScan)
      {
        adjustedTable.Append(table.ToStringCustomDecoration(header: true));
      }

      return adjustedTable.ToString();
    }
  }
}
