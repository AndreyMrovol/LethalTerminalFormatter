using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;

namespace TerminalFormatter.Nodes
{
    public class MoonsNoLLL : TerminalFormatterNode
    {
        public MoonsNoLLL()
            : base("Moons", ["MoonsCatalogue"]) { }

        public override bool IsNodeValid(TerminalNode node, Terminal terminal)
        {
            return true;
        }

        public override string GetNodeText(TerminalNode node, Terminal terminal)
        {
            Plugin.logger.LogDebug("Patching MoonsCatalogue");

            var table = new ConsoleTables.ConsoleTable(
                "", // Name
                "", // Price
                "" // Weather
            );

            var adjustedTable = new StringBuilder();

            string headerName = "MOONS CATALOGUE";
            string moonsHeader = new Header().CreateNumberedHeader(headerName, 2);

            List<TerminalFormatter.Route> routes = Variables
                .Routes.Where(keyval => keyval.Nodes.Node != null)
                .ToList()
                // order routes by Settings.MoonsOrderVanilla names
                .OrderBy(keyval =>
                    Settings.MoonsOrderVanilla.IndexOf(
                        MrovLib.API.SharedMethods.GetNumberlessPlanetName(keyval.Level)
                    )
                )
                .ToList();

            int itemCount = 1;

            foreach (TerminalFormatter.Route route in routes)
            {
                SelectableLevel level = route.Level;

                if (ConfigManager.AlwaysDisplayHiddenMoons.Value == false)
                {
                    if (
                        Settings.HiddenMoons.Contains(level.PlanetName)
                        || Settings.MoonsToIgnore.Contains(level.PlanetName)
                    )
                    {
                        continue;
                    }
                }
                else
                {
                    if (Settings.MoonsToIgnore.Contains(level.PlanetName))
                    {
                        continue;
                    }
                }

                int price = SharedMethods.GetPrice(route.Nodes.Node.itemCost);

                table.AddRow(
                    MrovLib
                        .API.SharedMethods.GetNumberlessPlanetName(level)
                        .PadRight(Settings.planetNameWidth),
                    $"${price}",
                    SharedMethods.GetWeather(level).PadRight(Settings.planetWeatherWidth)
                );

                if (itemCount % 3 == 0)
                {
                    itemCount = 1;
                    table.AddRow("", "", "");
                }
                else
                {
                    itemCount++;
                }
            }

            adjustedTable.Append(moonsHeader);
            adjustedTable.Append(
                $" The Company // Buying at {Mathf.RoundToInt(StartOfRound.Instance.companyBuyingRate * 100f)}% \n\n"
            );
            adjustedTable.Append(table.ToStringCustomDecoration());
            return adjustedTable.ToString();
        }
    }
}
