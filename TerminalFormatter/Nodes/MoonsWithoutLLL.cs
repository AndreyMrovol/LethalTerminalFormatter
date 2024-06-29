using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HarmonyLib;
using UnityEngine;

namespace TerminalFormatter.Nodes
{
    public class MoonsNoLLL : TerminalFormatterNode
    {
        public MoonsNoLLL()
            : base("Moons", ["MoonsCatalogue"])
        {
            this.AdditionalInfo =
                " Welcome to the exomoons catalogue! \n Use ROUTE to set the autopilot. \n Use INFO to learn about a moon.";
        }

        public override bool IsNodeValid(TerminalNode node, Terminal terminal)
        {
            return true;
        }

        public override string GetNodeText(TerminalNode node, Terminal terminal)
        {
            var table = new ConsoleTables.ConsoleTable(
                "Name", // Name
                "Price", // Price
                "Weather" // Weather
            );

            var adjustedTable = new StringBuilder();

            string headerName = "MOONS CATALOGUE";
            string moonsHeader = new Header().CreateNumberedHeader(headerName, 2);

            bool decor = ConfigManager.ShowDecorations.Value;

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

            if (decor)
            {
                table.AddRow("", "", "");
            }

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

                string moonName = ConfigManager.ShowNumberedPlanetNames.Value
                    ? level.PlanetName
                    : MrovLib.API.SharedMethods.GetNumberlessPlanetName(level);

                if (decor)
                {
                    moonName = $"* {moonName}";
                }

                table.AddRow(
                    moonName,
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

            adjustedTable.Append(this.AdditionalInfo != null ? $"{this.AdditionalInfo}\n\n" : "");

            adjustedTable.Append(
                $" The Company is buying at {Mathf.RoundToInt(StartOfRound.Instance.companyBuyingRate * 100f)}%. \n\n"
            );

            adjustedTable.Append(table.ToStringCustomDecoration(header: true));

            return adjustedTable.ToString();
        }
    }
}
