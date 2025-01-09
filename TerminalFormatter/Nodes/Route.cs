using System.Linq;
using System.Text;
using MrovLib;

namespace TerminalFormatter.Nodes
{
  public class Route : TerminalFormatterNode
  {
    public Route()
      : base("Route", ["route", "Route"]) { }

    public MrovLib.ContentType.Route ResolveNodeIntoRoute(TerminalNode node)
    {
      return ContentManager.Routes.Where(x => x.Nodes.Node == node).FirstOrDefault();
    }

    public override bool IsNodeValid(TerminalNode node, Terminal terminal)
    {
      MrovLib.ContentType.Route resolvedRoute = ResolveNodeIntoRoute(node);

      return resolvedRoute != null;
    }

    public override string GetNodeText(TerminalNode node, Terminal terminal)
    {
      var table = new ConsoleTables.ConsoleTable("Title", "Things");

      MrovLib.ContentType.Route resolvedRoute = ResolveNodeIntoRoute(node);

      var header = new Header().CreateHeaderWithoutLines("CONFIRM ROUTE");
      var adjustedTable = new StringBuilder();

      SelectableLevel currentLevel = resolvedRoute.Level;
      string currentWeather = SharedMethods.GetWeather(currentLevel);

      int price = SharedMethods.GetPrice(node.itemCost);

      table.AddRow("PLANET:", SharedMethods.GetNumberlessPlanetName(currentLevel));
      table.AddRow("PRICE:", $"${price} (${terminal.groupCredits - price} after routing)");
      table.AddRow("WEATHER:", currentWeather == "" ? "Clear" : currentWeather);

      // table.AddRow("", "");

      // table.AddRow("SIZE:", $"{currentLevel.factorySizeMultiplier}x");
      // table.AddRow(
      //     "SCRAP:",
      //     $"${currentLevel.minTotalScrapValue} ({currentLevel.minScrap}) - ${currentLevel.maxTotalScrapValue} ({currentLevel.maxScrap})"
      // );

      adjustedTable.Append(header);
      adjustedTable.Append("\n\n");
      adjustedTable.Append("Please CONFIRM or DENY routing the autopilot:");
      adjustedTable.Append("\n\n");
      adjustedTable.Append(table.ToStringCustomDecoration());

      return adjustedTable.ToString();
    }
  }
}
