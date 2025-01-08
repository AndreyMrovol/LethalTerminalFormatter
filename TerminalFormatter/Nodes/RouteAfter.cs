using System.Linq;
using System.Text;

namespace TerminalFormatter.Nodes
{
    public class RouteAfter : TerminalFormatterNode
    {
        public RouteAfter()
            : base("RouteAfter", ["route", "Route"]) { }

        public MrovLib.ItemHelper.Route ResolveNodeIntoRoute(TerminalNode node)
        {
            return MrovLib
                .ContentManager.Routes.Where(x => x.Nodes.NodeConfirm == node)
                .FirstOrDefault();
        }

        public override bool IsNodeValid(TerminalNode node, Terminal terminal)
        {
            MrovLib.ItemHelper.Route resolvedRoute = ResolveNodeIntoRoute(node);

            return resolvedRoute != null;
        }

        public override string GetNodeText(TerminalNode node, Terminal terminal)
        {
            var table = new ConsoleTables.ConsoleTable("Title", "Things");

            MrovLib.ItemHelper.Route resolvedRoute = ResolveNodeIntoRoute(node);

            var header = new Header().CreateHeaderWithoutLines("SUCCESS!");
            var adjustedTable = new StringBuilder();

            SelectableLevel currentLevel = resolvedRoute.Level;
            string currentWeather = SharedMethods.GetWeather(currentLevel);

            int price = SharedMethods.GetPrice(node.itemCost);

            table.AddRow("PLANET:", SharedMethods.GetNumberlessPlanetName(currentLevel));
            table.AddRow("WEATHER:", currentWeather == "" ? "Clear" : currentWeather);

            adjustedTable.Append(header);
            adjustedTable.Append("\n\n");
            adjustedTable.Append("Thank you for your purchase!\n");
            adjustedTable.Append("You're currently headed to:");
            adjustedTable.Append("\n\n");
            adjustedTable.Append(table.ToStringCustomDecoration());

            return adjustedTable.ToString();
        }
    }
}
