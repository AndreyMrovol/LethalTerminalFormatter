using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;

namespace TerminalFormatter
{
    partial class Nodes
    {
        public string MoonsNoLLL(TerminalNode node, Terminal terminal)
        {
            Plugin.logger.LogDebug("Patching MoonsCatalogue");
            Plugin.logger.LogWarning("LethalLevelLoader not found, using a fallback method");

            List<SelectableLevel> moonCatalogue = terminal.moonsCatalogueList.ToList();

            var table = new ConsoleTables.ConsoleTable(
                "", // Name
                "", // Price
                "" // Weather
            );

            Dictionary<string, SelectableLevel> numberlessMoons = [];
            moonCatalogue.Do(x =>
                numberlessMoons.Add(
                    new string(x.PlanetName.SkipWhile(c => !char.IsLetter(c)).ToArray()),
                    x
                )
            );

            List<List<Dictionary<string, int>>> groups =
                new()
                {
                    new List<Dictionary<string, int>>
                    {
                        new Dictionary<string, int> { { "Experimentation", 0 } },
                        new Dictionary<string, int> { { "Assurance", 0 } },
                        new Dictionary<string, int> { { "Vow", 0 } }
                    },
                    new List<Dictionary<string, int>>
                    {
                        new Dictionary<string, int> { { "March", 0 } },
                        new Dictionary<string, int> { { "Offense", 0 } }
                    },
                    new List<Dictionary<string, int>>
                    {
                        new Dictionary<string, int> { { "Rend", 550 } },
                        new Dictionary<string, int> { { "Dine", 600 } },
                        new Dictionary<string, int> { { "Titan", 700 } }
                    }
                };

            var adjustedTable = new StringBuilder();

            string headerName = "MOONS CATALOGUE";
            string moonsHeader = new Header().CreateNumberedHeader(headerName, 2);

            logger.LogDebug("MoonsCataloguePage: " + moonCatalogue);

            foreach (var group in groups)
            {
                foreach (var moonDictionary in group)
                {
                    foreach (var moonInDictionary in moonDictionary)
                    {
                        SelectableLevel moon = numberlessMoons[moonInDictionary.Key];

                        table.AddRow(
                            moonInDictionary.Key.PadRight(planetNameWidth),
                            $"${moonInDictionary.Value.ToString()}",
                            moon.currentWeather.ToString().PadRight(planetWeatherWidth)
                        );
                    }
                }

                table.AddRow("", "", "");
            }

            adjustedTable.Append(moonsHeader);
            adjustedTable.Append(RemoveTable(table.ToMarkDownString()));
            return adjustedTable.ToString();
        }
    }
}
