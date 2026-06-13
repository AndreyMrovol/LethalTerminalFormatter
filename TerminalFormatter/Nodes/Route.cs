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
      return ContentManager.Routes.FirstOrDefault(x => x.Nodes.Node == node);
    }

    public override bool IsNodeValid(TerminalNode node)
    {
      MrovLib.ContentType.Route resolvedRoute = ResolveNodeIntoRoute(node);

      return resolvedRoute != null;
    }

    public override string GetNodeText(TerminalNode node)
    {
      var table = new ConsoleTables.ConsoleTable("Title", "Things");

      MrovLib.ContentType.Route resolvedRoute = ResolveNodeIntoRoute(node);

      var header = new Header().CreateHeaderWithoutLines("CONFIRM ROUTE");
      var adjustedTable = new StringBuilder();

      SelectableLevel currentLevel = resolvedRoute.Level;
      string currentWeather = SharedMethods.GetWeather(currentLevel);

      int price = SharedMethods.GetPrice(node.itemCost);

      table.AddRow("PLANET:", SharedMethods.GetNumberlessPlanetName(currentLevel));
      table.AddRow("PRICE:", $"${price} (${Variables.Terminal.groupCredits - price} after routing)");
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
