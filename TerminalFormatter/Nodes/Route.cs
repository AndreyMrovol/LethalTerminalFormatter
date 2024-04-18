using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using BepInEx.Logging;
using HarmonyLib;
using LethalLevelLoader;
using UnityEngine;

namespace TerminalFormatter.Nodes
{
    public class Route : TerminalFormatterNode
    {
        public Route()
            : base("Route", ["route", "Route"]) { }

        public override bool IsNodeValid(TerminalNode node, Terminal terminal)
        {
            return node.buyRerouteToMoon == -2;
        }

        public override string GetNodeText(TerminalNode node, Terminal terminal)
        {
            var table = new ConsoleTables.ConsoleTable("Title", "Things");

            Plugin.logger.LogInfo("Creating route table");

            var header = new Header().CreateHeaderWithoutLines("CONFIRM ROUTE");
            var adjustedTable = new StringBuilder();

            List<SelectableLevel> levels = SharedMethods.GetGameLevels();
            SelectableLevel currentLevel = levels
                .Where(level =>
                    node.displayText.Contains(SharedMethods.GetNumberlessPlanetName(level))
                )
                .FirstOrDefault();

            // get all terminal nodes containing `route`, and find the one having numberless planet name in the description
            // oh god, it's so shit



            string currentWeather = SharedMethods.GetWeather(currentLevel);

            if (
                currentWeather.Length > Settings.terminalWidth - 5
                || ConfigManager.UseShortenedWeathers.Value
            )
            {
                Settings.WeathersShortened.Do(pair =>
                {
                    currentWeather = Regex.Replace(currentWeather, pair.Key, pair.Value);
                });
            }

            int price = SharedMethods.GetPrice(node.itemCost);

            table.AddRow("PLANET:", SharedMethods.GetNumberlessPlanetName(currentLevel));
            table.AddRow("PRICE:", $"${price} (${terminal.groupCredits - price} after routing)");
            table.AddRow("WEATHER:", currentWeather == "" ? "Clear" : currentWeather);

            adjustedTable.Append(header);
            adjustedTable.Append("\n\n");
            adjustedTable.Append("Please CONFIRM or DENY routing the autopilot:");
            adjustedTable.Append("\n\n");
            adjustedTable.Append(table.ToStringCustomDecoration());

            return adjustedTable.ToString();
        }
    }
}
