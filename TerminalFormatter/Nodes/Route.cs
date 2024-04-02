using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using BepInEx.Logging;
using HarmonyLib;
using LethalLevelLoader;
using UnityEngine;

namespace TerminalFormatter
{
    partial class Nodes
    {
        public string Route(TerminalNode node, Terminal terminal)
        {
            var table = new ConsoleTables.ConsoleTable(
                "Planet", // Name
                "Price", // Price
                "Weather" // Weather
            );

            var header = new Header().CreateHeaderWithoutLines("CONFIRM ROUTE");
            var adjustedTable = new StringBuilder();

            List<ExtendedLevel> levels = LethalLevelLoader.PatchedContent.ExtendedLevels;
            ExtendedLevel currentLevel = levels
                .Where(level => level.routeNode == node)
                .FirstOrDefault();

            string currentWeather = typeof(TerminalManager)
                .GetMethod("GetWeatherConditions", BindingFlags.NonPublic | BindingFlags.Static)
                .Invoke(null, [currentLevel.selectableLevel])
                .ToString()
                .Replace("(", "")
                .Replace(")", "");

            table.AddRow(
                currentLevel.NumberlessPlanetName.PadRight(planetNameWidth),
                "$" + currentLevel.RoutePrice,
                currentWeather == "" ? "Clear" : currentWeather
            );

            adjustedTable.Append(header);
            adjustedTable.Append("\n\n");
            adjustedTable.Append("Please CONFIRM or DENY routing the autopilot:");
            adjustedTable.Append("\n\n");
            adjustedTable.Append(table.ToStringCustomDecoration());

            return adjustedTable.ToString();
        }
    }
}
